using UnityEngine;
using Worldreaver.Loading;

public class TestLoading : MonoBehaviour
{
    public Loader loader;
    public static TestLoading instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        loader.InitializeTips(new []
        {
            "Tipppppppppppppppppppp A",
            "Tipppppppppppppppppppp B",
            "Tipppppppppppppppppppp C",
            "Tipppppppppppppppppppp D",
            "Tipppppppppppppppppppp E",
        });
    }
}
