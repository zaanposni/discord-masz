<?php


namespace App\Controller;


use App\Config\Config;
use App\Form\CreateCaseFormType;
use App\Helpers\Helpers;
use Exception;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\Routing\Annotation\Route;
use mikemccabe\JsonPatch\JsonPatch;

class ModCasePatchController extends AbstractController
{

    /**
     * @Route("/modcases/{guildid}/{caseid}/edit", requirements={"guildid"="[0-9]{18}", "caseid"="\d*"})
     */
    public function patchCase($guildid, $caseid, Request $request)
    {
        // url e.g. "/modcase/209557077343993856/new"
        // probably validate userid and redirect to other page if not valid

        $userInfo = Helpers::GetCurrentUser($_COOKIE);
        $logged_in_user = $userInfo;
        if (is_null($userInfo)) {
            return $this->render('index.html.twig', [
                'error' => [
                    'messages' => ['Failed to fetch user info or login invalid.']
                ]
            ]);
        }

        try {
            $navdata = Helpers::GetNavbarStaticData();
        } catch (Exception $e) {
            $navdata = [];
        }

        // get current modcase
        $client = HttpClient::create();
        $errorMessages = [];
        try {
            $url = Config::getAPIBaseURL().'/api/v1/modcases/'.$guildid.'/'.$caseid;
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
        } catch(Exception $e) {
            $errorMessages[] = 'Failed to load modcase.';
            $modCase = null;
        }
        $form = $this->createForm(CreateCaseFormType::class, $modCase);
        $form->handleRequest($request);
        if($form->isSubmitted() && $modCase != null) {
            $data = $form->getData();

            $sendNotificationRaw = $data['sendNotification'] ?? true;
            $sendNotification = $sendNotificationRaw ? 'true' : 'false';
            unset($data['sendNotification']);  // unset as this attribute is not expected by the api
            unset($modCase['labels']);  // unset so the php lib uses operation "replace" in json patch document

            $patchDocument = JsonPatch::diff($modCase, $data);

            $url = Config::getAPIBaseURL().'/api/v1/modcases/'.$guildid.'/'.$caseid; // change api url here
            $response = $client->request(
                'PATCH',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        'Content-Type' => 'application/json',
                        'Connection' => 'keep-alive',
                        'Content-Length' => strlen(json_encode($patchDocument))
                    ],
                    'body' => json_encode($patchDocument),
                    'query' => [
                        'sendNotification' => $sendNotification
                    ]
                ]
            );
            $statusCode = $response->getStatusCode();

            if ($statusCode === 200) {
                return $this->redirect('/modcases/'.$guildid.'/'.$caseid);
            }

            $errorMessages[] = 'Failed to patch ModCase. Response from API: '.$statusCode;
            $errorMessages[] = $response->getContent(false);
            return $this->render('modcase/patch.html.twig', [
                'guildid' => $guildid,
                'logged_in_user' => $logged_in_user,
                'navdata' => $navdata,
                'createCaseForm' => $form->createView(),
                'modcase' => $modCase,
                'tabtitle' => 'MASZ: Patch ModCase',
                'error' => [
                    'messages' => $errorMessages
                ]
            ]);
        }

        return $this->render('modcase/patch.html.twig', [
            'guildid' => $guildid,
            'now' => date("Y-m-d\\TH:i:s.u"),
            'logged_in_user' => $logged_in_user,
            'navdata' => $navdata,
            'createCaseForm' => $form->createView(),
            'modcase' => $modCase,
            'tabtitle' => 'MASZ: Patch ModCase',
            'error' => [
                'messages' => $errorMessages
            ]
        ]);

    }
}