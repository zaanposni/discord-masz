<?php


namespace App\Controller;

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

        try {
            $userInfo = Helpers::GetCurrentUser($_COOKIE);
            $logged_in_user = $userInfo;
            if (is_null($userInfo)) {
                return $this->render('index.html.twig', [
                    'error' => [
                        'messages' => ['Failed to fetch user info or login invalid.']
                    ]
                ]);
            }

            $statusCode = 'None';
            $client = HttpClient::create();
            $url = 'http://127.0.0.1:5565/api/v1/modcases/' . $guildid . '/all'; // change api url here
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                    ],
                ]
            );
            $statusCode = $response->getStatusCode();
            $modCasesContent = $response->getContent();
            $modCases = json_decode($modCasesContent, true);

            $url = 'http://127.0.0.1:5565/api/v1/discord/guilds/' . $guildid; // change api url here
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                    ],
                ]
            );
            $statusCode = $response->getStatusCode();
            $guildContent = $response->getContent();
            $guild = json_decode($guildContent, true);

            $filteredModCases = [];
            if (empty($_GET)) {
                $filteredModCases = $modCases;
            } else {
                foreach ($modCases as $modCase) {
                    foreach ($_GET as $key => $value) {
                        if ($modCase[$key] == $value) {
                            $filteredModCases[] = $modCase;
                        }
                    }
                }
            }
        } catch (Exception $e) {
            return $this->render('modcase/show.html.twig', [
                'modcases' => [],
                'logged_in_user' => $logged_in_user,
                'error' => [
                    'messages' => ['Failed to load modcases. Statuscode from API: ' . $statusCode]
                ]
            ]);
        }

        return $this->render('modcase/show.html.twig', [
            'modcases' => $filteredModCases,
            'guild' => $guild,
            'guildid' => $guild['id'],
            'logged_in_user' => $logged_in_user
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

        try {
            $userInfo = Helpers::GetCurrentUser($_COOKIE);
            $logged_in_user = $userInfo;
            if (is_null($userInfo)) {
                return $this->render('index.html.twig', [
                    'error' => [
                        'messages' => ['Failed to fetch user info or login invalid.']
                    ]
                ]);
            }

            // create api request
            $statusCode = 'None';
            $errorMessages = [];
            $client = HttpClient::create();
            $url = 'http://127.0.0.1:5565/api/v1/modcases/' . $guildid . "/" . $id; // change api url here
            $response = $client->request(
                'GET',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                    ],
                ]
            );
            $statusCode = $response->getStatusCode();
            $modCaseContent = $response->getContent();
            $modCase = json_decode($modCaseContent, true);

            try {
                $url = 'http://127.0.0.1:5565/api/v1/discord/guilds/' . $modCase["guildId"]; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $statusCode = $response->getStatusCode();
                $guildContent = $response->getContent();
                $guild = json_decode($guildContent, true);
                $guildid = $guild['id'];
            } catch (Exception $e) {
                $guild = null;
                $guildid = null;
                $errorMessages[] = 'Failed to load detailed info about guild';
            }

            try {
                $url = 'http://127.0.0.1:5565/api/v1/discord/users/' . $modCase["modId"]; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $statusCode = $response->getStatusCode();
                $moderatorContent = $response->getContent();
                $moderator = json_decode($moderatorContent, true);
            } catch (Exception $e) {
                $moderator = null;
                $errorMessages[] = 'Failed to load detailed info about moderator';
            }

            try {
                $url = 'http://127.0.0.1:5565/api/v1/discord/users/' . $modCase["lastEditedByModId"]; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $statusCode = $response->getStatusCode();
                $lastModeratorContent = $response->getContent();
                $lastModerator = json_decode($lastModeratorContent, true);
            } catch (Exception $e) {
                $lastModerator = null;
                $errorMessages[] = 'Failed to load detailed info about last moderator';
            }

            try {
                $url = 'http://127.0.0.1:5565/api/v1/discord/users/' . $modCase["userId"]; // change api url here
                $response = $client->request(
                    'GET',
                    $url,
                    [
                        'headers' => [
                            'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        ],
                    ]
                );
                $statusCode = $response->getStatusCode();
                $userContent = $response->getContent();
                $user = json_decode($userContent, true);
            } catch (Exception $e) {
                $user = null;
                $errorMessages[] = 'Failed to load detailed user info';
            }

            if (count($errorMessages)) {
                return $this->render('modcase/view.html.twig', [
                    'modcase' => $modCase,
                    'guild' => $guild,
                    'guildid' => $guildid,
                    'moderator' => $moderator,
                    'lastModerator' => $lastModerator,
                    'user' => $user,
                    'logged_in_user' => $logged_in_user,
                    'error' => [
                        'messages' => $errorMessages
                    ]
                ]);
            }

            return $this->render('modcase/view.html.twig', [
                'modcase' => $modCase,
                'guild' => $guild,
                'guildid' => $guildid,
                'moderator' => $moderator,
                'lastModerator' => $lastModerator,
                'logged_in_user' => $logged_in_user,
                'user' => $user
            ]);
        } catch (Exception $e) {
            return $this->render('modcase/view.html.twig', [
                'error' => [
                    'messages' => ['Failed to load modcase. Statuscode from API: ' . $statusCode]
                ]
            ]);
        }
    }

}