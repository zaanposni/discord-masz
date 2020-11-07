<?php


namespace App\Controller;


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

        $form = $this->createForm(RegisterGuildFormType::class);

        $form->handleRequest($request);

        if($form->isSubmitted()) {
            $data = $form->getData();

            $client = HttpClient::create();
            $url = Config::getAPIBaseURL().'/api/v1/configs/' . $data['guildid'];
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
                ]
            );

            $statusCode = $response->getStatusCode();

            if ($statusCode === 201) {
                return $this->redirect('/modcases/'. $data['guildid']);
            }

            $errorMessages = [];
            $errorMessages[] = 'Failed to create ModCase. Response from API: '.$statusCode;
            $errorMessages[] = $response->getContent(false);
            return $this->render('guilds/new.html.twig', [
                'logged_in_user' => $logged_in_user,
                'navdata' => $navdata,
                'form' => $form->createView(),
                'tabtitle' => 'MASZ: New Guild',
                'error' => [
                    'messages' => $errorMessages
                ]
            ]);
        }

        return $this->render('guilds/new.html.twig', [
            'logged_in_user' => $logged_in_user,
            'navdata' => $navdata,
            'form' => $form->createView(),
            'tabtitle' => 'MASZ: New Guild'
        ]);
    }
}