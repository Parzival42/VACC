using UnityEngine;
using System.Collections;

public class VacuumSoundSimulation : MonoBehaviour {

    /// <summary>
    /// True if the Vacuumcleaner is turned on.
    /// </summary>
    public bool activated = false;

    [Tooltip("Controls value of the vacuum cleaners power. (0.0 - 1.0)")]
    [Range(0.0f, 1.0f)]
    public float power = 0.5f;

    [Tooltip("Controls the damping of the power change. (0.0 - 1.0)")]
    [Range(0.0f, 1.0f)]
    public float damping = 0.0f;

    /// <summary>
    /// The actual power, to simulate acceleration of rotor and blockage. (0.0 - 1.0)
    /// </summary>
    private float power_real = 0.0f;

    /// <summary>
    /// Defines how much percent the fuzzy sets (in this case the samples) overlap
    /// </summary>
    private float overlap = 1.0f;

    [Tooltip("The degree of nozzle coverage. (0.0 - 1.0)")]
    [Range(0.0f, 1.0f)]
    public float blockage = 0.0f;

    /// <summary>
    /// 20ms samples of the different sucking states.
    /// </summary>
    public AudioClip[] samples;
    [Range(0.0f, 1.0f)]

    /// <summary>
    /// Amplitude of the samples
    /// </summary>
    public float[] sampleWeight;

    private AudioSource audioSource;

    ///    1  2  3  4  5
    ///   ‾‾\ /\ /\ /\ /‾‾
    ///      \  \  \  \
    ///   __/ \/ \/ \/ \__
    /// 
    ///   Graph of sample weight with 5 samples and overlap of 100% (x - weight, y - power)

    void Start () {
        audioSource = GetComponent<AudioSource>();
        // load samples
    }
	
	void Update () {
        power_real = power;

        // reset weights
        for(int i = 0; i < sampleWeight.Length; i++)
        {
            sampleWeight[i] = 0;
        }

        // calculate index of neighbor samples
        float interpolation = (samples.Length - 1) * power_real;
        int indexLower = (int) Mathf.Floor(interpolation);
        int indexUpper = (int) Mathf.Ceil(interpolation);

        // calculate weight of samples
        sampleWeight[indexLower] = 1 - (interpolation - indexLower);
        sampleWeight[indexUpper] = 1 - (indexUpper - interpolation);

        // play samples
        if (!audioSource.isPlaying)
        {
            audioSource.clip = samples[indexLower];
            audioSource.volume = sampleWeight[indexLower];
            audioSource.Play();
        }
        
    }
}
