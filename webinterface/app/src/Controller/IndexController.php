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
            $guilds = array();
            foreach ($userInfo['guilds'] as &$guildid) {
                $url = Config::GetBaseUrl().'/api/v1/discord/guilds/' . $guildid; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $guildContent = $response->getContent();
                $guild = json_decode($guildContent, true);  // TODO: handle error

                $url = Config::GetBaseUrl().'/api/v1/stats/' . $guildid; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $statsContent = $response->getContent();
                $stats = json_decode($statsContent, true);  // TODO: handle error

                $guilds[] = [
                    'obj' => $guild,
                    'stats' => $stats
                ];
            }
            return $this->render('guilds/show.html.twig', [
                'guilds' => $guilds,
                'logged_in_user' => $logged_in_user,
                'tabtitle' => 'MASZ: Your guilds'
            ]);
        } catch (Exception $e) {
            return $this->render('guilds/show.html.twig', [
                'guilds' => [],
                'logged_in_user' => $logged_in_user,
                'error' => [
                    'messages' => ['Failed to fetch guild info.']
                ]
            ]);
        }



    }

}