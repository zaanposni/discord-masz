<?php

namespace App\API;

use App\Config\Config;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class Response
{
    public $success;
    public $statuscode;
    public $body;
    public $rawbody;

    function __construct($success, $statuscode = null, $body = null) {
        $this->success = $success;
        $this->statuscode = $statuscode;
        try {
            $this->body = json_decode($body, true);
        } catch(Exception $e) {
            $this->body = null;
        }
        $this->rawbody = $body;
    }

    function toString() {
        if ($this->success) {
            if (is_null($this->body)) {
                return $this->statuscode.': '.$this->rawbody;
            }
            if (is_array($this->body)) {
                $msg = json_encode($this->body);
            } else {
                $msg = strval($this->body);
            }
            return $this->statuscode.': '.$msg;
        }
        return 'Failed API call.';
    }
}