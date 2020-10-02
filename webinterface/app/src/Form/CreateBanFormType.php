<?php


namespace App\Form;

use Symfony\Component\Form\AbstractType;
use Symfony\Component\Form\Extension\Core\Type\SubmitType;
use Symfony\Component\Form\Extension\Core\Type\TextareaType;
use Symfony\Component\Form\Extension\Core\Type\TextType;
use Symfony\Component\Form\FormBuilderInterface;
use Symfony\Component\Form\Extension\Core\Type\ChoiceType;
use Symfony\Component\OptionsResolver\OptionsResolver;



class CreateBanFormType extends AbstractType
{

    public function buildForm(FormBuilderInterface $builder, array $options)
    {

        $builder
            ->add('userid', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Discord UserID',
                'disabled' => true,
            ])
            ->add('username', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Username',
            ])
            ->add('nickname', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Nickname',
            ])
            ->add('title', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Title',
            ])
            ->add('label', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Labels',
            ])
            ->add('description', TextareaType::class, [
                'attr' => [
                    'class' => "form-control md-textarea",
                ],
                'label' => 'Description',
            ])
            ->add('severity', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Severity',
            ])
            ->add('occuredAt', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Date & time',
            ])
            ->add('duration', TextType::class, [
                'attr' => [
                    'class' => "form-control",
                ],
                'label' => 'Banduration',
            ])
            ->add('submit', SubmitType::class, [
                'label' => 'CREATE BANCASE',
                'attr' => [
                    'class' => 'btn btn-danger btn-lg btn-block'
                ]
            ])
        ;
    }

}