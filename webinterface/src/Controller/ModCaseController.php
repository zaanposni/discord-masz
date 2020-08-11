<?php


namespace App\Controller;


use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\Routing\Annotation\Route;

class ModCaseController
{

    /**
     * @Route("/modcase/")
     */
    public function listAll() {

        // create api request
        $client = HttpClient::create();
        $url = ''; // change api url here
        $response = $client->request(
            'GET',
            $url
        );
        $statusCode = $response->getStatusCode();
        $content = $response->getContent();
        $content = $response->toArray();

        return $this->render('base.html.twig');
    }

    /**
     * @Route("/modcase/createcase")
     */
    public function createCase() {

        // create api request
        $client = HttpClient::create();
        $url = ''; // change api url here
        $casedata = [];
        $response = $client->request('POST', $url, [
            'json' => $casedata,
        ]);

        $decodedPayload = $response->toArray();


        return $this->render('base.html.twig');
    }

    /**
     * @Route("/modcase/{id}")
     */
    public function showCase($id) {

        // create api request
        $client = HttpClient::create();
        $url = ''.$id; // change api url here
        $response = $client->request(
            'GET',
            $url
        );
        $statusCode = $response->getStatusCode();
        $content = $response->getContent();
        $content = $response->toArray();

        return $this->render('base.html.twig');
    }

}