using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityManager))]
public class GameManager : MonoBehaviour
{
    /// <summary>The singleton instance of the GameManager</summary>
    public static GameManager instance { get; private set; }

    #region References
    public GravityManager gravityManager { get; private set; }
    #endregion References

    #region Variables

    #endregion Variables

    private void Awake()
    {
        if (instance == null)
        {
            //Make Singleton
            instance = this;
            DontDestroyOnLoad(gameObject);

            //Assign references first thing in the game.
            AssignReferences();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeVariables();
    }

    /// <summary>
    /// Assign any references, so any variables in those references can be initialized in Start().
    /// </summary>
    private void AssignReferences()
    {
        gravityManager = GetComponent<GravityManager>();
    }

    /// <summary>
    /// Initialize any variables in the game.
    /// </summary>
    private void InitializeVariables()
    {
        
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
