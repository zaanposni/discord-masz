<?php


namespace App\Controller;


use App\Form\CreateBanFormType;
use App\Form\CreateMuteFormType;
use App\Form\CreateNoteFormType;
use App\Form\CreateWarnFormType;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\Routing\Annotation\Route;

class ModCaseCreationController extends AbstractController
{

    /**
     * @Route("/modcase/{userid}/new/ban", name="create_ban")
     */
    public function createBanCase($userid) {

        // url e.g. "/modcase/209557077343993856/new/ban"
        // probably validate userid and redirect to other page if not valid

        $form = $this->createForm(CreateBanFormType::class);

        return $this->render('modcase/new/ban.html.twig', [
        'userid' => $userid,
        'createBanForm' => $form->createView(),
        ]);
    }

    /**
     * @Route("/modcase/{userid}/new/mute", name="create_mute")
     */
    public function createMuteCase($userid) {

        // url e.g. "/modcase/209557077343993856/new/mute"
        // probably validate userid and redirect to other page if not valid

        $form = $this->createForm(CreateMuteFormType::class);

        return $this->render('modcase/new/mute.html.twig', [
            'userid' => $userid,
            'createMuteForm' => $form->createView(),
        ]);
    }

    /**
     * @Route("/modcase/{userid}/new/warn", name="create_warn")
     */
    public function createWarnCase($userid) {

        // url e.g. "/modcase/209557077343993856/new/mute"
        // probably validate userid and redirect to other page if not valid

        $form = $this->createForm(CreateWarnFormType::class);

        return $this->render('modcase/new/warn.html.twig', [
            'userid' => $userid,
            'createWarnForm' => $form->createView(),
        ]);
    }

    /**
     * @Route("/modcase/{userid}/new/note", name="create_note")
     */
    public function createNoteCase($userid) {

        // url e.g. "/modcase/209557077343993856/new/note"
        // probably validate userid and redirect to other page if not valid

        $form = $this->createForm(CreateNoteFormType::class);

        return $this->render('modcase/new/note.html.twig', [
            'userid' => $userid,
            'createNoteForm' => $form->createView(),
        ]);
    }

}