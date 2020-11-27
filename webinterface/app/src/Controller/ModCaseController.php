<?php


namespace App\Controller;

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

class ModCaseController extends AbstractController
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
            $basicData->errors[] = 'Failed to fetch user info or login invalid.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $modCases = ModCaseAPI::SelectAll($_COOKIE, $guildid);
        if (!$modCases->success || is_null($modCases->body) || $modCases->statuscode !== 200) {
            $basicData->errors[] = 'Failed to load modcases. API: ';
            $basicData->errors[] = $modCases->toString();
            return $this->render('modcase/view.html.twig', [
                'basic_data' => $basicData
            ]);
        }
        $modCases = $modCases->body;
        $guild = DiscordAPI::GetGuild($_COOKIE, $guildid)->body;

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

        $basicData->tabTitle = 'MASZ: '.$guild['name'].': ModCases';
        return $this->render('modcase/list.html.twig', [
            'basic_data' => $basicData,
            'modcases' => $filteredModCases,
            'activePunishments' => $activePunishmentCases,
            'guild' => $guild
        ]);
    }


    /**
     * @Route("/modcases/{guildid}/{id}", requirements={"guildid"="[0-9]{18}", "id"="\d*"})
     */
    public function showCase($guildid, $id)
    {
        if (!isset($_COOKIE["masz_access_token"])) {
            return $this->render('index.html.twig');
        }

        $basicData = new BasicData($_COOKIE);
        $basicData->currentGuild = $guildid;
        if (is_null($basicData->loggedInUser)) {
            $basicData->errors[] = 'Failed to fetch user info or login invalid.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $modCase = ModCaseAPI::Select($_COOKIE, $guildid, $id);
        if (!$modCase->success || is_null($modCase->body) || $modCase->statuscode !== 200) {
            $basicData->errors[] = 'Failed to load modcase. API: ';
            $basicData->errors[] = $modCase->toString();
            return $this->render('modcase/view.html.twig', [
                'basic_data' => $basicData
            ]);
        }
        $modCase = $modCase->body;

        $guild = DiscordAPI::GetGuild($_COOKIE, $guildid)->body;
        if (is_null($guild)) {
            $basicData->errors[] = 'Failed to load detailed info about guild';
        }

        $moderator = DiscordAPI::GetUser($_COOKIE, $modCase['modId'])->body;
        if (is_null($moderator)) {
            $basicData->errors[] = 'Failed to load detailed info about moderator';
        }

        $lastModerator = DiscordAPI::GetUser($_COOKIE, $modCase['lastEditedByModId'])->body;
        if (is_null($lastModerator)) {
            $basicData->errors[] = 'Failed to load detailed info about last moderator';
        }

        $caseUser = DiscordAPI::GetUser($_COOKIE, $modCase['userId'])->body;
        if (is_null($caseUser)) {
            $basicData->errors[] = 'Failed to load detailed user info';
        }

        $newComments = [];  // comments with discord user object merged
        $fetchedUser = [];
        foreach ($modCase['comments'] as $comment) {
            if (!array_key_exists($comment['userId'], $fetchedUser)) {
                $comment['discordUser'] = DiscordAPI::GetUser($_COOKIE, $comment['userId'])->body;
            } else {
                $comment['discordUser'] = $fetchedUser[$comment['userId']];
            }
            $newComments[] = $comment;
        }
        $modCase['comments'] = $newComments;

        $basicData->tabTitle = 'MASZ: #'.$modCase['caseId'].': '.$modCase['title'];
        return $this->render('modcase/view.html.twig', [
            'basic_data' => $basicData,
            'modcase' => $modCase,
            'guild' => $guild,
            'moderator' => $moderator,
            'lastModerator' => $lastModerator,
            'user' => $caseUser
        ]);
    }
}