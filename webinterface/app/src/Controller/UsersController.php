<?php


namespace App\Controller;


use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\Routing\Annotation\Route;

class UsersController extends AbstractController
{

    /**
     * @Route("/users/")
     */
    public function listAll() {

        // create api request
//        $client = HttpClient::create();
//        $url = ''; // change api url here
//        $response = $client->request(
//            'GET',
//            $url
//        );
//        $statusCode = $response->getStatusCode();
//        $content = $response->getContent();
//        $content = $response->toArray();

        return $this->render('users/show.html.twig');
    }


}