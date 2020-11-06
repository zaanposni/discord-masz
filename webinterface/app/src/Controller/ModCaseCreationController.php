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

class ModCaseCreationController extends AbstractController
{

    /**
     * @Route("/modcases/{guildid}/new", requirements={"guildid"="[0-9]{18}"})
     */
    public function createCase($guildid, Request $request) {
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
        } catch(Exception $e) {
            $navdata = [];
        }

        $form = $this->createForm(CreateCaseFormType::class);

        $form->handleRequest($request);

        if($form->isSubmitted()) {
            $data = $form->getData();

            $sendNotificationRaw = $data['sendNotification'] ?? true;
            $sendNotification = $sendNotificationRaw ? 'true' : 'false';

            $client = HttpClient::create();
            $url = Config::getAPIBaseURL().'/api/v1/modcases/' . $guildid; // change api url here
            $response = $client->request(
                'POST',
                $url,
                [
                    'headers' => [
                        'Cookie' => 'masz_access_token=' . $_COOKIE["masz_access_token"],
                        'Content-Type' => 'application/json',
                        'Connection' => 'keep-alive',
                        'Content-Length' => strlen(json_encode($data))
                    ],
                    'body' => json_encode($data),
                    'query' => [
                        'sendNotification' => $sendNotification
                    ]
                ]
            );
            $statusCode = $response->getStatusCode();

            if ($statusCode === 201) {
                $responseContent = $response->getContent();
                $created = json_decode($responseContent, true);
                $id = $created['caseid'];
                return $this->redirect('/modcases/'.$guildid.'/'.$id);
            }

            $errorMessages = [];
            $errorMessages[] = 'Failed to create ModCase. Response from API: '.$statusCode;
            $errorMessages[] = $response->getContent(false);
            return $this->render('modcase/new/case.html.twig', [
                'guildid' => $guildid,
                'now' => 'test',
                'logged_in_user' => $logged_in_user,
                'navdata' => $navdata,
                'createCaseForm' => $form->createView(), // TODO: error handling,
                'error' => [
                    'messages' => $errorMessages
                ]
            ]);
        }

        return $this->render('modcase/new/case.html.twig', [
            'guildid' => $guildid,
            'now' => date("Y-m-d\\TH:i:s.u"),
            'logged_in_user' => $logged_in_user,
            'navdata' => $navdata,
            'createCaseForm' => $form->createView(),
            'tabtitle' => 'MASZ: New ModCase'
        ]);
    }
}