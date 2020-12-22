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
    public function patchCase($guildid, $caseid)
    {
        if (!isset($_COOKIE["masz_access_token"])) {
            return $this->render('index.html.twig');
        }

        $basicData = new BasicData($_COOKIE);
        $basicData->currentGuild = $guildid;
        if (is_null($basicData->loggedInUser)) {
            $basicData->errors[] = 'You have been logged out.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $modCase = ModCaseAPI::Select($_COOKIE, $guildid, $caseid)->body;
        if (is_null($modCase)) {
            $basicData->errors[] = 'Failed to load modcase.';
        }

        return $this->render('modcase/patch.html.twig', [
            'basic_data' => $basicData,
            'modcase' => $modCase
        ]);
    }
}
