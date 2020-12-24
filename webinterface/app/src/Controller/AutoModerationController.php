<?php

namespace App\Controller;

use App\API\AutoModerationConfigAPI;
use App\API\DiscordAPI;
use App\API\GuildConfigAPI;
use App\API\ModCaseAPI;
use App\Helpers\BasicData;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\Routing\Annotation\Route;

class AutoModerationController extends AbstractController
{

    /**
     * @Route("/automoderations/{guildid}", requirements={"guildid"="[0-9]{18}"})
     */
    public function patchGuild($guildid)
    {
        if (!isset($_COOKIE["masz_access_token"])) {
            return $this->render('index.html.twig');
        }

        $basicData = new BasicData($_COOKIE);
        if (is_null($basicData->loggedInUser)) {
            $basicData->errors[] = 'You have been logged out.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $guild = DiscordAPI::GetGuild($_COOKIE, $guildid)->body;
        if (is_null($guild)) {
            $basicData->errors[] = 'Failed to load current guild info.';
        }

        $guildChannels = DiscordAPI::GetGuildChannels($_COOKIE, $guildid)->body;
        if (is_null($guildChannels)) {
            $basicData->errors[] = 'Failed to load current guild channels info.';
        }
        $guild["channels"] = $guildChannels;

        $config = AutoModerationConfigAPI::SelectAll($_COOKIE, $guildid)->body;
        if (is_null($config)) {
            $basicData->errors[] = 'Failed to load current configs.';
        }

        foreach ($guild["roles"] as $i => $role) {
            $guild["roles"][$i]["color"] = "#" . dechex($role["color"]);
        }

        $basicData->tabTitle = "MASZ: Edit automod config";
        return $this->render('automoderation/configuration.html.twig', [
            'basic_data' => $basicData,
            'guild' => $guild,
            'config' => $config
        ]);
    }
}
