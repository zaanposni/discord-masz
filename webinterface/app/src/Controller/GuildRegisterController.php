<?php


namespace App\Controller;


use App\API\DiscordAPI;
use App\API\MetaAPI;
use App\Helpers\BasicData;
use App\API\GuildConfigAPI;
use App\Config\Config;
use App\Form\CreateCaseFormType;
use App\Form\RegisterGuildFormType;
use App\Helpers\Helpers;
use Exception;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\Routing\Annotation\Route;

class GuildRegisterController extends AbstractController
{

    /**
     * @Route("/newguild")
     */
    public function registerGuild(Request $request) {
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

        $guilds = DiscordAPI::GetGuilds($_COOKIE);
        if (!$guilds->success || is_null($guilds->body) || $guilds->statuscode !== 200) {
            $basicData->tabTitle = 'MASZ: New Guild';
            $basicData->errors[] = "Failed to fetch guilds info. API:" . $guilds->toString();
            return $this->render('guilds/new.html.twig', [
                'basic_data' => $basicData,
            ]);
        }

        $clientid = MetaAPI::GetClientId($_COOKIE);
        if (!$clientid->success || is_null($clientid->body) || $clientid->statuscode !== 200) {
            $basicData->errors[] = "Failed to fetch meta infos. API:";
            $basicData->errors[] = $clientid->toString();
            return $this->render('guilds/new.html.twig', [
                'basic_data' => $basicData,
                'guilds' => $guilds->body
            ]);
        }

        $basicData->tabTitle = 'MASZ: New Guild';
        return $this->render('guilds/new.html.twig', [
            'basic_data' => $basicData,
            'guilds' => $guilds->body,
            'client_id' => $clientid->body
        ]);
    }
}
