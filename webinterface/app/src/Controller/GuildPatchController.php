<?php

namespace App\Controller;

use App\API\GuildConfigAPI;
use App\API\ModCaseAPI;
use App\Helpers\BasicData;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\Routing\Annotation\Route;

class GuildPatchController extends AbstractController
{

    /**
     * @Route("/{guildid}/edit", requirements={"guildid"="[0-9]{18}"})
     */
    public function patchGuild($guildid)
    {
        if (!isset($_COOKIE["masz_access_token"])) {
            return $this->render('index.html.twig');
        }

        $basicData = new BasicData($_COOKIE);
        if (is_null($basicData->loggedInUser)) {
            $basicData->errors[] = 'Failed to fetch user info or login invalid.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $guildConfig = GuildConfigAPI::Select($_COOKIE, $guildid)->body;
        if (is_null($guildConfig)) {
            $basicData->errors[] = 'Failed to load current guild config.';
        }

        $basicData->tabTitle = "MASZ: Edit guild";
        return $this->render('guilds/edit.html.twig', [
            'basic_data' => $basicData,
            'guildconfig' => $guildConfig
        ]);
    }
}