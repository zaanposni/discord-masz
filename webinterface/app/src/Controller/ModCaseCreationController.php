<?php


namespace App\Controller;


use App\Form\CreateCaseFormType;
use App\Helpers\Helpers;
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

        $form = $this->createForm(CreateCaseFormType::class);

        $form->handleRequest($request);

        if($form->isSubmitted()) {
            $data = $form->getData();

            $client = HttpClient::create();
            $url = 'http://127.0.0.1:5565/api/v1/modcases/' . $guildid; // change api url here
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
                    'body' => json_encode($data)
                ]
            );
            $statusCode = $response->getStatusCode();
            var_dump($statusCode);

            if ($statusCode === 201) {
                $modCasesContent = $response->getContent();
                $created = json_decode($modCasesContent, true);
                $id = $created['id'];
                return $this->redirect('/modcases/'.$guildid.'/'.$id);
            }

            return $this->render('modcase/new/case.html.twig', [
                'guildid' => $guildid,
                'now' => 'test',
                'createCaseForm' => $form->createView(), // TODO: error handling
            ]);
        }

        return $this->render('modcase/new/case.html.twig', [
            'guildid' => $guildid,
            'now' => date("d.m.Y H:i"),
            'logged_in_user' => $logged_in_user,
            'createCaseForm' => $form->createView(),
        ]);
    }
}