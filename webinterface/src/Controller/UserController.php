<?php


namespace App\Controller;


use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;

class UserController extends AbstractController
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
    public function show(Request $request, $slug)
    {
        $client = HttpClient::create();
        $url = "https://api.github.com/repos/symfony/symfony-docs";
        $response = $client->request(
            'GET',
            $url
        );
        $statusCode = $response->getStatusCode();
        $content = $response->getContent();
        $content = $response->toArray();
        return $this->render('users/show.html.twig', [
            'datas' => $content
        ]);
    }

}