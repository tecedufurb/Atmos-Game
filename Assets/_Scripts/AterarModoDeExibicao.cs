﻿using UnityEngine;

public class AterarModoDeExibicao : MonoBehaviour {

    private static bool isVisaoDeCima = true;
    public GameObject visaoCimaObjetosMundo;
    public GameObject visaoCimaCanvas;
    public GameObject visao1PessoaObjetosMundo;
    public GameObject visao1PessoaCanvas;
    public GameObject rio;
    
    public void onClickMudarVisao() {
        if (isVisaoDeCima) { //se esta na visao de cima
            GameManager.podePlantar = false;
            visao1PessoaObjetosMundo.active = true; //ativa os objetos do mundo da visao do personagem
            visao1PessoaCanvas.active = true; //ativa o canvas da visao do personagem
            visaoCimaCanvas.active = false; //desativa canvas visao de cima
            visaoCimaObjetosMundo.active = false; //desativa objetos do mundo da visao de cima
            isVisaoDeCima = false; //desativar visao de cima
            rio.GetComponent<AudioSource>().enabled = true;
        } else {
            GameManager.podePlantar = true;
            visaoCimaObjetosMundo.active = true; //ativa os objetos do mundo da visao de cima
            visaoCimaCanvas.active = true; //ativa o canvas da visao de cima
            visao1PessoaCanvas.active = false; //desativa canvas visao do personagem
            visao1PessoaObjetosMundo.active = false; //desativa objetos mundo visao do personagem
            isVisaoDeCima = true; //ativa a visao de cima
            rio.GetComponent<AudioSource>().enabled = false;
        }
    }
}