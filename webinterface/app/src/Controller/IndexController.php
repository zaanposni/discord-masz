<?php


namespace App\Controller;


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

        $userInfo = Helpers::GetCurrentUser($_COOKIE);
        $logged_in_user = $userInfo;
        if (is_null($userInfo)) {
            return $this->render('index.html.twig', [
                'error' => [
                    'messages' => ['Failed to fetch user info or login invalid.']
                ]
            ]);
        }

        $client = HttpClient::create();
        try {
            $navdata = Helpers::GetNavbarStaticData();
        } catch(Exception $e) {
            $navdata = [];
        }
        try {
            $modGuilds = array();
            $memberGuilds = array();
            $bannedGuilds = array();
            foreach ($userInfo['modGuilds'] as $guildid) {
                $modGuilds[] = [
                    'obj' => Helpers::GetGuildById($guildid, $_COOKIE),
                    'stats' => Helpers::GetGuildStatsById($guildid, $_COOKIE)
                ];
            }
            foreach ($userInfo['memberGuilds'] as $guildid) {
                $memberGuilds[] = [
                    'obj' => Helpers::GetGuildById($guildid, $_COOKIE),
                    'stats' => Helpers::GetGuildStatsById($guildid, $_COOKIE)
                ];
            }
            foreach ($userInfo['bannedGuilds'] as $guildid) {
                $bannedGuilds[] = [
                    'obj' => Helpers::GetGuildById($guildid, $_COOKIE),
                    'stats' => Helpers::GetGuildStatsById($guildid, $_COOKIE)
                ];
            }
            return $this->render('guilds/show.html.twig', [
                'mod_guilds' => $modGuilds,
                'member_guilds' => $memberGuilds,
                'banned_guilds' => $bannedGuilds,
                'logged_in_user' => $logged_in_user,
                'tabtitle' => 'MASZ: Your guilds',
                'navdata' => $navdata
            ]);
        } catch (Exception $e) {
            return $this->render('guilds/show.html.twig', [
                'guilds' => [],
                'logged_in_user' => $logged_in_user,
                'error' => [
                    'messages' => ['Failed to fetch guild info.']
                ],
                'navdata' => $navdata
            ]);
        }



    }

}