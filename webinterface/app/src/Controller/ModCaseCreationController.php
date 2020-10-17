<?php


namespace App\Controller;


use App\Form\CreateCaseFormType;
use App\Helpers\Helpers;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\Routing\Annotation\Route;

class ModCaseCreationController extends AbstractController
{

    /**
     * @Route("/modcases/{guildid}/new", requirements={"guildid"="[0-9]{18}"})
     */
    public function createCase($guildid) {
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

        return $this->render('modcase/new/case.html.twig', [
            'guildid' => $guildid,
            'now' => date("d.m.Y H:i"),
            'logged_in_user' => $logged_in_user,
            'createCaseForm' => $form->createView(),
        ]);
    }
}