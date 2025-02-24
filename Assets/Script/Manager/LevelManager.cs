using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [DisableInEditorMode]
    public List<Customer> listAllCustomer = new();


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    private void Start()
    {
       // ReviewList();
    }
    public void ReviewList()
    {
        if (listAllCustomer.Count > 0)
        {
            listAllCustomer.RemoveAll(item => item == null);
        }
    }
    public void DestroyCustomer(int quantity, ColorType color)
    {
        StartCoroutine(DestroyCustomerCoroutine(quantity, color));
    }
    IEnumerator DestroyCustomerCoroutine(int quantity, ColorType color)
    {
        int i = 0;
        for (int j = 0; j < listAllCustomer.Count; j++)
        {
            if (listAllCustomer[j].customerColor == color)
            {
                Instantiate(SH_GameManager.Instance._VFX_Confetti, listAllCustomer[j].gameObject.transform.position, Quaternion.identity);
                Destroy(listAllCustomer[j].gameObject);
                i++;
                if (i == quantity) break;
                yield return new WaitForSeconds(0.3f);
            }
        }
        yield return new WaitForSeconds(0.5f);
        ReviewList();
        CheckWin(null);
        ObserverManager.ReviewGate();
        SH_GameManager.Instance._canPressDestroy = true;

    }
    public void CheckWin(Customer? customer)
    {
        if (customer != null)
        {
            listAllCustomer.Remove(customer);
        }
;
       if(listAllCustomer.Count == 0)
        {
            Victory();
        }
    }

    public void Victory()
    {
        Debug.Log("Chuc mung ban da thang !");
        ObserverManager.Victory(true);
        //Invoke(nameof(NextLevelTest),1f);
    }
    private void NextLevelTest()
    {
        SH_GameManager.Instance.ShowGOPanel();
        Destroy(gameObject);
    }
    public void Defeat()
    {
        Time.timeScale = 0f;
       // SH_GameManager.Instance.ShowGOPanel();
        Debug.Log("Con ga con !");
    }
}
