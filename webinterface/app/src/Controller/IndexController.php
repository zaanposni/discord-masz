<?php


namespace App\Controller;


use App\Helpers\BasicData;
use App\API\DiscordAPI;
use App\API\GuildConfigAPI;
use App\API\StatsAPI;
use App\Config\Config;
use App\Helpers\Helpers;
use Exception;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\Routing\Annotation\Route;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;


class IndexController extends AbstractController
{
    /**
     * @Route("/")
     */
    public function index() {
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

            $modGuilds = array();
            $memberGuilds = array();
            $bannedGuilds = array();
            foreach ($basicData->loggedInUser['modGuilds'] as $guildid) {
                $modGuilds[] = [
                    'obj' => DiscordAPI::GetGuild($_COOKIE, $guildid)->body,
                    'stats' => StatsAPI::Select($_COOKIE, $guildid)->body
                ];
            }
            foreach ($basicData->loggedInUser['memberGuilds'] as $guildid) {
                $memberGuilds[] = [
                    'obj' => DiscordAPI::GetGuild($_COOKIE, $guildid)->body,
                    'stats' => StatsAPI::Select($_COOKIE, $guildid)->body
                ];
            }
            foreach ($basicData->loggedInUser['bannedGuilds'] as $guildid) {
                $bannedGuilds[] = [
                    'obj' => DiscordAPI::GetGuild($_COOKIE, $guildid)->body,
                    'stats' => StatsAPI::Select($_COOKIE, $guildid)->body
                ];
            }

            $basicData->tabTitle = 'MASZ: Your guilds';
            return $this->render('guilds/list.html.twig', [
                'basic_data' => $basicData,
                'mod_guilds' => $modGuilds,
                'member_guilds' => $memberGuilds,
                'banned_guilds' => $bannedGuilds
            ]);

    }
}