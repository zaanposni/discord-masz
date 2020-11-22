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



class CreateCaseFormType extends AbstractType
{

    public function buildForm(FormBuilderInterface $builder, array $options)
    {
        $builder
            ->add('userId', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Discord UserId'
            ])
            ->add('guildId', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Discord GuildId',
                'disabled' => true,
            ])
            ->add('title', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Title',
            ])
            ->add('labels', CollectionType::class, [
                'entry_options' => [
                    'attr' => ['class' => 'label-box list-group-item'],
                    'label' => false
                ],
                'allow_add' => true,
                'allow_delete' => true,
                'label' => false,
                'by_reference' => false,
                'prototype' => true,
                'entry_type' => TextType::class,
            ])
            ->add('description', TextareaType::class, [
                'attr' => [
                    'class' => "form-control md-textarea",
                ],
                'label' => 'Description'
            ])
            ->add('severity', ChoiceType::class, [
                'attr' => [
                    'class' => "form-control browser-default custom-select",
                ],
                'choices' => [
                    'Medium' => 0,
                    'Normal' => 1,
                    'Hard' => 2,
                    'Epic' => 3
                ],
                'data' => 1,
                'label' => 'Severity'
            ])
            ->add('occuredAt', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Date & time',
                'empty_data' => date("Y-m-d\\TH:i:s.u"),
                'required'=> false
            ])
            ->add('punishment', ChoiceType::class, [
                'attr' => [
                    'class' => "form-control browser-default custom-select",
                ],
                'choices' => [
                    'Warn' => 'Warn',
                    'Mute' => 'Mute',
                    'TempMute' => 'TempMute',
                    'Kick' => 'Kick',
                    'Ban' => 'Ban',
                    'TempBan' => 'TempBan',
                    'Notice' => 'Notice',
                    'None' => 'None'
                ],
                'label' => 'Punishment',
            ])
            ->add('sendNotification', CheckboxType::class, [

                'attr' => [
                    'class' => "custom-control-input",
                    'checked' => true
                ],
                'label' => 'Send notification?',
                'label_attr' => array(
                    'class' => 'custom-control-label'
                ),
                'required'=> false
            ])
            ->add('submit', SubmitType::class, [
                'label' => 'CREATE CASE',
                'attr' => [
                    'class' => 'btn btn-danger btn-lg btn-block'
                ]
            ])
        ;
    }

}