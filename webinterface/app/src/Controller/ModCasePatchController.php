<?php


namespace App\Controller;


use App\Helpers\BasicData;
use App\API\ModCaseAPI;
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

        $modCase = ModCaseAPI::Select($_COOKIE, $guildid, $caseid)->body;
        if (is_null($modCase)) {
            $basicData->errors[] = 'Failed to load modcase.';
        }

        $form = $this->createForm(CreateCaseFormType::class, $modCase);
        $form->handleRequest($request);
        if($form->isSubmitted()) {
            $data = $form->getData();

            $sendNotificationRaw = $data['sendNotification'] ?? true;
            $sendNotification = $sendNotificationRaw ? 'true' : 'false';
            unset($data['sendNotification']);  // unset as this attribute is not expected by the api

            $response = ModCaseAPI::Update($_COOKIE, $guildid, $caseid, $data, $sendNotification);

            if ($response->success && $response->statuscode === 200) {
                return $this->redirect('/modcases/'.$guildid.'/'.$caseid);
            }

            $basicData->errors[] = 'Failed to patch ModCase. Response from API: ';
            $basicData->errors[] = $response->toString();
            $basicData->tabTitle = 'MASZ: Patch ModCase';
            $basicData->currentGuild = $guildid;
            return $this->render('modcase/patch.html.twig', [
                'basic_data' => $basicData,
                'createCaseForm' => $form->createView(),
                'now' => date("Y-m-d\\TH:i:s.u"),
                'modcase' => $modCase
            ]);
        }

        return $this->render('modcase/patch.html.twig', [
            'basic_data' => $basicData,
            'createCaseForm' => $form->createView(),
            'now' => date("Y-m-d\\TH:i:s.u"),
            'modcase' => $modCase
        ]);

    }
}