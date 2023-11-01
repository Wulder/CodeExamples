using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public class testdll : MonoBehaviour
{
    [SerializeField] private int[] arr;

    [DllImport("testdll", EntryPoint = "TestSort")]
    static extern void TestSort(int[] a, int length);

    [ContextMenu("sort")]
    void Sort()
    {
        TestSort(arr, arr.Length);
    }

}
