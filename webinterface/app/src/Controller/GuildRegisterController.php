<?php


namespace App\Controller;


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
            $basicData->errors[] = 'Failed to fetch user info or login invalid.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $form = $this->createForm(RegisterGuildFormType::class);
        $form->handleRequest($request);

        if($form->isSubmitted()) {
            $data = $form->getData();
            $response = GuildConfigAPI::Post($_COOKIE, $data['guildid'], $data);

            if ($response->success) {
                if ($response->statuscode === 201) {
                    return $this->redirect('/modcases/'. $data['guildid']);
                }
            }

            $basicData->tabTitle = 'MASZ: New Guild';
            $basicData->errors[] = 'Failed to create ModCase. Response from API: ';
            $basicData->errors[] = $response->toString();
            return $this->render('guilds/new.html.twig', [
                'basic_data' => $basicData,
                'form' => $form->createView(),
            ]);
        }

        $basicData->tabTitle = 'MASZ: New Guild';
        return $this->render('guilds/new.html.twig', [
            'basic_data' => $basicData,
            'form' => $form->createView(),
        ]);
    }
}