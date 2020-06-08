using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGenerator : MonoBehaviour {

    /// <summary>
    /// Objeto que possui o Collider
    /// </summary>
    [Header("Objects Reference")]
    [Tooltip("Informa o Objeto que possui o Collider")]
    public GameObject colliderObject;

    /// <summary>
    /// Objeto que vai ser instancia no chão
    /// </summary>
    [Tooltip("Informa o Objeto que vai ser instanciado no chão")]
    public GameObject ground;

    /// <summary>
    /// Objeto que vai ser instanciado nos andares acima do chão
    /// </summary>
    [Tooltip("Informa o Objeto que vai ser instanciado nos andares acima do chão")]
    public GameObject upperFloors;

    /// <summary>
    /// Comprimento do Objeto (X)
    /// </summary>
    [Header("Object Aspects")]
    [Tooltip("Comprimento do Objeto (X)")]
    public float blockWScale;

    /// <summary>
    /// Altura do Objeto (Y)
    /// </summary>
    [Tooltip("Altura do Objeto (Y)")]
    public float blockHScale;

    /// <summary>
    /// Profundidade do Objeto (Z)
    /// </summary>
    [Tooltip("Profundidade do Objeto (Z)")]
    public float blockDScale;

    /// <summary>
    /// Comprimento do prédio
    /// </summary>
    [Header("Building aspects")]
    [Tooltip("Comprimento do Prédio (X)")]
    public int width;

    /// <summary>
    /// Altura do Prédio
    /// </summary>
    [Tooltip("Altura do Prédio (X)")]
    public int height;

    /// <summary>
    /// Profundidade do Prédio
    /// </summary>
    [Tooltip("Profundidade do Prédio (X)")]
    public int depth;

    /// <summary>
    /// Informa se está tudo OK
    /// </summary>
    public bool OK {
        get {
            return width > 0 &&
                height > 0 &&
                depth > 0 &&
                colliderObject != null &&
                ground != null &&
                upperFloors != null;
        }
    }

    /// <summary>
    /// Antes de Inicializar
    /// </summary>
    void Awake() {
        if (!OK) {
            this.gameObject.SetActive(false);
            return;
        }

        if (this.blockWScale == 0)
            this.blockWScale = this.colliderObject.transform.localScale.x;
        if (this.blockHScale == 0)
            this.blockHScale = this.colliderObject.transform.localScale.y;
        if (this.blockDScale == 0)
            this.blockDScale = this.colliderObject.transform.localScale.z;

        GameLogic.Instance.RegisterBlockBuilds(this);
    }

    /// <summary>
    /// Cria os Blocos predefinidos
    /// </summary>
    public void Create() {

        GameObject Generated = new GameObject();
        Generated.name = "Factory";
        Generated.transform.position = Vector3.zero;
        Vector3 position = Generated.transform.position;

        //Frente
        Vector3 aux = position;
        int door = Random.Range(0, width);

        for (int i = 0; i < width; i++) {

            bool isFloor = true;


            for (int j = 0; j < height; j++) {
                aux += new Vector3(0, blockHScale, 0);
                GameObject go = Instantiate(colliderObject, aux, Generated.transform.rotation, Generated.transform);

                if (isFloor) {
                    if (door == i) {
                        Instantiate(ground, go.transform);
                    }
                    else {
                        Instantiate(ground, go.transform.position, Quaternion.Euler(Generated.transform.rotation.eulerAngles + new Vector3(0, 180, 0)), go.transform);
                    }

                    isFloor = false;
                }
                else
                    Instantiate(upperFloors, go.transform);

            }

            aux = new Vector3(aux.x, position.y, aux.z);
            aux += new Vector3(blockWScale, 0, 0);
        }

        //Atrás
        aux = new Vector3(position.x, position.y, blockDScale * (depth - 1) + position.z);
        door = Random.Range(0, width);

        for (int i = 0; i < width; i++) {
            bool isFloor = true;
            for (int j = 0; j < height; j++) {
                aux += new Vector3(0, blockHScale, 0);
                GameObject go = Instantiate(colliderObject, aux, Generated.transform.rotation, Generated.transform);

                if (isFloor) {

                    if (door != i) {
                        Instantiate(ground, go.transform);
                    }
                    else {
                        Instantiate(ground, go.transform.position, Quaternion.Euler(Generated.transform.rotation.eulerAngles + new Vector3(0, 180, 0)), go.transform);
                    }
                    isFloor = false;
                }
                else
                    Instantiate(upperFloors, go.transform);
            }

            aux = new Vector3(aux.x, position.y, aux.z);
            aux += new Vector3(blockWScale, 0, 0);
        }

        //Lado Esquerdo
        aux = new Vector3(position.x, position.y, position.z + blockDScale);

        for (int i = 0; i < depth - 2; i++) {
            bool isFloor = true;
            for (int j = 0; j < height; j++) {
                aux += new Vector3(0, blockHScale, 0);
                GameObject go = Instantiate(colliderObject, aux, Generated.transform.rotation, Generated.transform);

                if (isFloor) {
                    Instantiate(ground, go.transform);
                    isFloor = false;
                }
                else
                    Instantiate(upperFloors, go.transform);

            }

            aux = new Vector3(aux.x, position.y, aux.z);
            aux += new Vector3(0, 0, blockDScale);
        }

        //Lado Direito
        aux = new Vector3(blockDScale * (width - 1) + position.x, position.y, position.z + blockDScale);

        for (int i = 0; i < depth - 2; i++) {
            bool isFloor = true;

            for (int j = 0; j < height; j++) {
                aux += new Vector3(0, blockHScale, 0);
                GameObject go = Instantiate(colliderObject, aux, Generated.transform.rotation, Generated.transform);

                if (isFloor) {
                    Instantiate(ground, go.transform);
                    isFloor = false;
                }
                else
                    Instantiate(upperFloors, go.transform);
            }

            aux = new Vector3(aux.x, position.y, aux.z);
            aux += new Vector3(0, 0, blockDScale);
        }

        Generated.transform.SetParent(this.transform, false);
    }

}
