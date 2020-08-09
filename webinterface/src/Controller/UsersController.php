<?php


namespace App\Controller;


use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;

class UsersController extends AbstractController
{

    /**
     * @Route("/")
     */
    public function homepage() {
        return $this->render('base.html.twig');
    }

    /**
     * @Route("/users/{slug}")
     */
    public function show($slug) {
        return $this->render('users/show.html.twig', [
            'username' => $slug
        ]);
    }

}