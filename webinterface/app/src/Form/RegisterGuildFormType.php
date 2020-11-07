<?php


namespace App\Form;

use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\CheckboxType;
use Symfony\Component\Form\Extension\Core\Type\CollectionType;
use Symfony\Component\Form\Extension\Core\Type\EmailType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\Extension\Core\Type\TextareaType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\OptionsResolver\OptionsResolver;



class RegisterGuildFormType extends AbstractType
{

    public function buildForm(FormBuilderInterface $builder, array $options)
    {

        $builder
            ->add('guildid', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Guild Id'
            ])
            ->add('modroleid', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Role Id for moderators, these can see all cases.'
            ])
            ->add('adminroleid', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Role Id for guild admins, these can see all cases and do advanced configuration.'
            ])
            ->add('modPublicNotificationWebhook', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'empty_data' => null,
                'required'=> false,
                'label' => 'Webhook URL for public notifications, leave empty for no notifications'
            ])
            ->add('modInternalNotificationWebhook', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'empty_data' => null,
                'required'=> false,
                'label' => 'Webhook URL for internal notifications, leave empty for no notifications'
            ])
            ->add('submit', SubmitType::class, [
                'label' => 'REGISTER GUILD',
                'attr' => [
                    'class' => 'btn btn-danger btn-lg btn-block'
                ]
            ])
        ;
    }

}