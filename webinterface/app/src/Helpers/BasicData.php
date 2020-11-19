<?php

namespace App\API;

use App\Config\Config;
use App\Helpers\Helpers;
use Exception;
use Symfony\Component\HttpClient\HttpClient;

class BasicData
{
    public $loggedInUser;
    public $patchNotes;
    public $tabTitle;
    public $errors;
    public $token;
    public $currentGuild;

    function __construct($COOKIES) {
        $this->token = $COOKIES['masz_access_token'] ?? '';
        $this->loggedInUser = Helpers::GetCurrentUser($COOKIES);
        $this->patchNotes = Helpers::GetPatchNotes();
        $this->tabTitle = 'MASZ';
        $this->errors = [];
        $this->currentGuild = null;
    }
}
