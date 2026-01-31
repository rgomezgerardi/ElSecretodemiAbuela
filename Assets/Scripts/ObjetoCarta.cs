using System;
using UnityEngine;

[Serializable]
public class ObjetoCarta
{
    [SerializeField] private int valorCarta;

    public int ValorCarta => valorCarta;

    public void SetValor(int valor)
    {
        valorCarta = valor;
    }
}
