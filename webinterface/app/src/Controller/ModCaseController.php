<?php


namespace App\Controller;

use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\Routing\Annotation\Route;

class ModCaseController extends AbstractController
{

    /**
     * @Route("/modcase/")
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

        return $this->render('modcase/show.html.twig');
    }



    /**
     * @Route("/modcase/{id}")
     */
    public function showCase($id) {

        // create api request
//        $client = HttpClient::create();
//        $url = ''.$id; // change api url here
//        $response = $client->request(
//            'GET',
//            $url
//        );
//        $statusCode = $response->getStatusCode();
//        $content = $response->getContent();
//        $content = $response->toArray();

        return $this->render('modcase/view.html.twig', [
            'id' => $id,
        ]);
    }

}