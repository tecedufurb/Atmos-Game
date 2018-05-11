﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlantarArvores : MonoBehaviour {

    #region arraysComparacao
    private string[] naoDeveColidir = { "Agua", "Untagged", "PlantaDoMundo" };
    private string casoEspeciais = "PlantaDoMundo"; //se colidir com planta do mundo, deve pegar o objeto que colidiu e destruilo 
    private string[] deveColidir = { "AreaApp", "Terreno" }; //esse pode adicionado novas coisa
    private string terrenoTag = "Terreno";//esse é especifico para terreno    
    #endregion

    #region caracteristicasPlantar 
    private int numeroDePlantasParaInstanciar = 5;
    private int raioPlantas = 10;
    private float distanciaMaximaRay = 300;
    #endregion

    private static bool colidiuComApp;
    private static int areaApp = 1; //determina se colidiu com lado esquerdo ou direito do rio

    RaycastHit[] hitsInfoAux; //armazena o hit inicial
    private List<string> nomesPlantasParaInstanciar; //nomes das plantas selecionadas no painel

    private AudioManager mAudioManager;

    void Start() {
        mAudioManager = FindObjectOfType<AudioManager>();
    }

    private void OnEnable(){
        InputManager.OnTap += plantar;
    }

    private void OnDisable(){
          InputManager.OnTap -= plantar;
    }

    private void plantar(){
        if (GameManager.podePlantar && pegaPlantasDoPainel() != null) {
            Ray pointCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            hitsInfoAux = returnHitsOfRay(pointCameraRay); //Pega os HitsInfo do do 1º click
            nomesPlantasParaInstanciar = retornaNomePlantasParaInstanciar(pegaPlantasDoPainel());
            if (nomesPlantasParaInstanciar != null) { // se nomes não está vazio
                if (isFirstClickValid(hitsInfoAux)) { //se o 1 click nao colidiu com agua ou outro objeto untagged ou um painel
                    nomesPlantasParaInstanciar = retornaNomePlantasParaInstanciar(pegaPlantasDoPainel());//pega o nome das plantas selecionadas quando faz o click
                    validarPlantar(hitsInfoAux, pointCameraRay); //tentar plantar
                }
            }   
        }
    }

    private List<string> pegaPlantasDoPainel() { //pega o nome das plantas que estão selecionadas no canvas
        InformacoesBotao informacao; //armazena as informaçoes
        GameObject[] botoesCanvas = GameManager.botoesCanvas; //botoes do canvas
        List<string> nomesPlantasSelecionadas = new List<string>();//lista que armazena os nomes das plantas selecionadas
        for (int i = 0; i < botoesCanvas.Length; i++) { //seleciona o nome das plantas que estao selecionadas 
            informacao = botoesCanvas[i].GetComponent<InformacoesBotao>();
            if (informacao.estadoDoBotao) { //se o botao esta ativo
                nomesPlantasSelecionadas.Add(informacao.Imagem); //adiciona o nome a lista
            }
        }
        if (nomesPlantasSelecionadas.Count > 5) {
            return null;
            Debug.Log("Mais que 5 plantas selecionadas");
        }
        if (nomesPlantasSelecionadas.Count < 1) {
            return null;
            Debug.Log("Menos que 1 planta selecionada");
        }
        return nomesPlantasSelecionadas; //retorna o nome das plantas selecionadas ou null
    }
    private List<string> retornaNomePlantasParaInstanciar(List<string> plantasSelecionadas) { //esse metodo escolhe aleatoriamente as plantas que vao ser instanciadas, com base na lista recebida
        List<string> plantasParaInstanciar = new List<string>();//lista que armazena os nomes das plantas selecionadas
        int numeroPlantasSelecionadas = plantasSelecionadas.Count;//pega o numero de elementos da lista
        if (numeroPlantasSelecionadas > 0 && numeroPlantasSelecionadas < 6) {
            if (numeroPlantasSelecionadas == 1) {
                for (int i = 0; i < 5; i++) {
                    plantasParaInstanciar.Add(plantasSelecionadas[0]);//joga 5 vezes a planta na lista
                }
            }
            else if (numeroPlantasSelecionadas == 2) {
                plantasParaInstanciar.Add(plantasSelecionadas[0]);
                plantasParaInstanciar.Add(plantasSelecionadas[0]);
                plantasParaInstanciar.Add(plantasSelecionadas[1]);
                plantasParaInstanciar.Add(plantasSelecionadas[1]);
                int random = (int)Random.Range(0.0f, 1.9f);  //gera um random entre 0 e 1
                plantasParaInstanciar.Add(plantasSelecionadas[random]); //adiciona uma das duas plantas a lista
            }
            else if (numeroPlantasSelecionadas == 3) {
                plantasParaInstanciar.Add(plantasSelecionadas[0]);
                plantasParaInstanciar.Add(plantasSelecionadas[1]);
                plantasParaInstanciar.Add(plantasSelecionadas[2]);
                int random1 = (int)Random.Range(0.0f, 2.9f);  //gera um random entre 0 e 2
                plantasParaInstanciar.Add(plantasSelecionadas[random1]);
                while (true) { //nao pode gerar o mesmo numero random de cima
                    int random2 = (int)Random.Range(0.0f, 2.9f);  //gera um random entre 0 e 2
                    if (random2 != random1) { //random 2 nao deve ser igual a random 1, para que nao tenha 3 plantas do mesmo tipo
                        plantasParaInstanciar.Add(plantasSelecionadas[random2]);
                        break;
                    }
                }
            }
            else if (numeroPlantasSelecionadas == 4) {
                plantasParaInstanciar.Add(plantasSelecionadas[0]);
                plantasParaInstanciar.Add(plantasSelecionadas[1]);
                plantasParaInstanciar.Add(plantasSelecionadas[2]);
                plantasParaInstanciar.Add(plantasSelecionadas[3]);
                int random = (int)Random.Range(0.0f, 3.9f);  //gera um random entre 0 e 3
                plantasParaInstanciar.Add(plantasSelecionadas[random]);
            }
            else if (numeroPlantasSelecionadas == 5) {
                for (int i = 0; i < 5; i++) {
                    plantasParaInstanciar.Add(plantasSelecionadas[i]);//joga uma planta de cada na lista
                }
            }
            return plantasParaInstanciar; //retorna o nome das plantas para instanciar
        }
        else {
            return null;
        }
    }
    private bool isFirstClickValid(RaycastHit[] hitsInfo) { //Verifica se o raio também nao colidiu com UI
        if (!GameManager.IsPointerOverUIObject()) {//se nao colidiu com o botoes UI
            foreach (string tag in naoDeveColidir) {
                foreach (RaycastHit hitInfo in hitsInfo) {
                    if (hitInfo.transform.tag == tag) { //se uma das tags é igual a naoDevePlantas
                        return false; //nao é um ponto valido
                    }
                }
            }
            if (pegaPlantasDoPainel() != null) { //se tem planta selecionada
                return true;//pode plantar
            }
        }
        return false;
    }
   
    private void instanciaPlanta(RaycastHit hitToInstantiate, string nomePlantaParaInstanciar) { //instancia as plantas, passar somente os RayscastHits com o terreno
        var plantaPrefab = Resources.Load("Prefabs/" + nomePlantaParaInstanciar) as GameObject;//carrega prefab
        plantaPrefab = Instantiate(plantaPrefab, hitToInstantiate.point, new Quaternion(), GameObject.Find("PlantasDoTerreno").transform) as GameObject;//instancia planta
        plantaPrefab.tag = "PlantaDoMundo";
        plantaPrefab.GetComponent<DetalhePlantaMundo>().setValor(PontuacaoPlantas.setValorPlanta(nomePlantaParaInstanciar));
        nomesPlantasParaInstanciar.Remove(nomePlantaParaInstanciar);//remove o nome da lista de plantas para instanciar
        if (colidiuComApp) {  //se colidiu com alguma box
            PontuacaoPlantas.incrementaBox(hitToInstantiate.point, nomePlantaParaInstanciar, areaApp);
        }
    }
    private bool validaLocalPlanta(RaycastHit[] hitsInfo) { //se nao colidiu com naoDeveColidir
        colidiuComApp = false; ; //reset variavel
        foreach (RaycastHit hitInfo in hitsInfo) {
            foreach (string tag in naoDeveColidir) {
                if (hitInfo.transform.tag == tag) { //se uma das tags é igual a naoDevePlantas
                    return false; //nao é um ponto valido
                }
            }
        }
        foreach (RaycastHit hitInfo in hitsInfo) {
            if (hitInfo.transform.tag == "AreaApp1") {
                colidiuComApp = true;
                areaApp = 1;
            }
            if (hitInfo.transform.tag == "AreaApp2") {
                colidiuComApp = true;
                areaApp = 2;
            }
        }
        return true;
    }
    private RaycastHit[] returnHitsOfRay(Ray ray) {
        return Physics.RaycastAll(ray, distanciaMaximaRay);
    }
    private RaycastHit returnHitTerrain(RaycastHit[] hitsInfo) {
        foreach (RaycastHit hit in hitsInfo) {
            if (hit.transform.tag == terrenoTag) {
                return hit;//retorna o hitInfo do terreno
            }
        }
        Debug.Log("HITS in hitsInfo .transform.tag");
        foreach (RaycastHit hit in hitsInfo) {
            Debug.Log(hit.transform.tag);
        }
        throw new System.Exception("Nao encontrou colisao com terreno!----PANIC MODE ON!!  ");
        return new RaycastHit();
    }
    private Ray createNewRay(Ray origem) {
        Ray newRay = new Ray(new Vector3(Random.Range(origem.origin.x - raioPlantas, origem.origin.x + raioPlantas), origem.origin.y, Random.Range(origem.origin.z - raioPlantas, origem.origin.z + raioPlantas)), origem.direction);//gera raio com base na area definida em raioPlantas
        return newRay;
    }

    private void validarPlantar(RaycastHit[] hitsInfo, Ray rayOrigem) {
        Ray rayAuxiliar;
        RaycastHit hitToInstatiate;
        int numeroDeRaysValidos = 0; //ja comeca com 1 planta instanciada
        int numeroTentativas = 15;//ira tentar ate 15 vezes
        int countNomePlantaParaInstanciar = 4; //é um aux

        while (numeroDeRaysValidos < 5 && numeroTentativas > 0) { //se ainda nao tem 5 plantas geradas && ainda nao tentou 20 vezes 
            rayAuxiliar = createNewRay(rayOrigem); //cria um novo raio
            hitsInfoAux = returnHitsOfRay(rayAuxiliar);
            if (validaLocalPlanta(hitsInfoAux)) { //se local é valido
                hitToInstatiate = returnHitTerrain(hitsInfoAux);//prepara o hit para instanciar
                instanciaPlanta(hitToInstatiate, nomesPlantasParaInstanciar[countNomePlantaParaInstanciar]);//instancia a planta
                countNomePlantaParaInstanciar--;//passa pelos nomes da plantas
            }
            numeroTentativas--;
            numeroDeRaysValidos++;
        }
        mAudioManager.PlayPlantAudio();
    }
}