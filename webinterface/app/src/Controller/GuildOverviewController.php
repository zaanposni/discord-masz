<?php


namespace App\Controller;

use App\API\AutoModerationEventAPI;
use App\API\FilesAPI;
use App\Helpers\BasicData;
use App\API\CommentsAPI;
use App\API\DiscordAPI;
use App\API\GuildConfigAPI;
use App\API\ModCaseAPI;
use App\Config\Config;
use App\Helpers\Helpers;
use Exception;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\Routing\Annotation\Route;

class GuildOverviewController extends AbstractController
{
    /**
     * @Route("/modcases/{guildid}")
     */
    public function listAll($guildid)
    {
        if (!isset($_COOKIE["masz_access_token"])) {
            return $this->render('index.html.twig');
        }

        $basicData = new BasicData($_COOKIE);
        $basicData->currentGuild = $guildid;
        if (is_null($basicData->loggedInUser)) {
            $basicData->errors[] = 'You have been logged out.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $modCases = ModCaseAPI::SelectAll($_COOKIE, $guildid);
        if (!$modCases->success || is_null($modCases->body) || $modCases->statuscode !== 200) {
            $basicData->errors[] = 'Failed to load modcases. API: ' . $modCases->toString();
            $modCases->body = [];
        }
        $modCases = $modCases->body;
        $guild = DiscordAPI::GetGuild($_COOKIE, $guildid)->body;

        // filter modcases
        try {
            $filteredModCases = [];
            if (empty($_GET)) {
                $filteredModCases = $modCases;
            } else {
                foreach ($modCases as $modCase) {
                    foreach ($_GET as $key => $value) {
                        if ($key === 'label') {
                            if (in_array($value, $modCase['labels'])) {
                                $filteredModCases[] = $modCase;
                                break;
                            }
                        } else {
                            if ($modCase[$key] == $value) {
                                $filteredModCases[] = $modCase;
                                break;
                            }
                        }
                    }
                }
            }
        } catch (Exception $e) {
            $filteredModCases = [];
        }

        $activePunishmentCases = [];
        foreach ($modCases as $modCase) {
            if ($modCase['punishmentActive']) {
                $activePunishmentCases[] = $modCase;
            }
        }

        $autoModerations = AutoModerationEventAPI::Select($_COOKIE, $guildid);
        if (!$autoModerations->success || is_null($autoModerations->body) || $autoModerations->statuscode !== 200) {
            $basicData->errors[] = 'Failed to load automoderations. API: ' . $autoModerations->toString();
            $autoModerations->body = ['events' => [], 'count' => 0];
        }
        $autoModerations = $autoModerations->body;

        $basicData->tabTitle = 'MASZ: ' . $guild['name'] . ': ModCases';
        return $this->render('modcase/list.html.twig', [
            'basic_data' => $basicData,
            'modcases' => $filteredModCases,
            'activePunishments' => $activePunishmentCases,
            'autoModerations' => $autoModerations,
            'guild' => $guild
        ]);
    }
}
