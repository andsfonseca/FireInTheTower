using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBuild : MonoBehaviour {

    public GameObject block;
    public int width;
    public int height;
    public int depth;

    public GameObject floor;
    public GameObject wall;

    /// <summary>
    /// Comprimento do Block (X)
    /// </summary>
    public float blockWScale;

    /// <summary>
    /// Altura do Block (Y)
    /// </summary>
    public float blockHScale;

    /// <summary>
    /// Profundidade do Block (Z)
    /// </summary>
    public float blockDScale;

    /// <summary>
    /// Informa se está tudo OK
    /// </summary>
    public bool OK {
        get {
            return width > 0 &&
                height > 0 &&
                depth > 0 &&
                block != null &&
                floor != null &&
                wall != null;
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
            this.blockWScale = this.block.transform.localScale.x;
        if (this.blockHScale == 0)
            this.blockHScale = this.block.transform.localScale.y;
        if (this.blockDScale == 0)
            this.blockDScale = this.block.transform.localScale.z;

        GameLogic.Instance.RegisterBlockBuilds(this);
    }

    /// <summary>
    /// Cria os Blocos predefinidos
    /// </summary>
    public void Create() {
        Vector3 position = this.transform.position;

        //Frente
        Vector3 aux = position;
        int door = Random.Range(0, width);

        for (int i = 0; i < width; i++) {

            bool isFloor = true;
            

            for (int j = 0; j < height; j++) {
                aux += new Vector3(0, blockHScale, 0);
                GameObject go = Instantiate(block, aux, this.transform.rotation, this.transform);

                if (isFloor) {
                    if (door == i) {
                        Instantiate(floor, go.transform);
                    }
                    else {
                        Instantiate(floor, go.transform.position, Quaternion.Euler( this.transform.rotation.eulerAngles + new Vector3(0, 180, 0)) , go.transform);
                    }
                    
                    isFloor = false;
                }
                else
                    Instantiate(wall, go.transform);

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
                GameObject go = Instantiate(block, aux, this.transform.rotation, this.transform);

                if (isFloor) {

                    if (door != i) {
                        Instantiate(floor, go.transform);
                    }
                    else {
                        Instantiate(floor, go.transform.position, Quaternion.Euler(this.transform.rotation.eulerAngles + new Vector3(0, 180, 0)), go.transform);
                    }
                    isFloor = false;
                }
                else
                    Instantiate(wall, go.transform);
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
                GameObject go = Instantiate(block, aux, this.transform.rotation, this.transform);

                if (isFloor) {
                    Instantiate(floor, go.transform);
                    isFloor = false;
                }
                else
                    Instantiate(wall, go.transform);

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
                GameObject go = Instantiate(block, aux, this.transform.rotation, this.transform);

                if (isFloor) {
                    Instantiate(floor, go.transform);
                    isFloor = false;
                }
                else
                    Instantiate(wall, go.transform);
            }

            aux = new Vector3(aux.x, position.y, aux.z);
            aux += new Vector3(0, 0, blockDScale);
        }

    }

}
