using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using KeepCoding;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class ChineseRemainderTheoremScript : ModuleScript
{
    private KMBombModule _Module;
    private System.Random _Rnd;

    [SerializeField]
    private TextMesh _OutputDisplay, _Index, _Input;
    [SerializeField]
    internal KMSelectable _Display, _ClearButton, _SubmitButton;
    [SerializeField]
    internal KMSelectable[] _NumberButtons;

    private bool _isModuleSolved, _isSeedSet, _isAnswerCorrect = true;
    internal int _seed, _secret, _depth, _index, _lowestCommonMultiple;
    internal List<int> _moduli = new List<int>(), _remainder = new List<int>();

    // Use this for initialization
    private void Start()
    {
        if (!_isSeedSet)
        {
            _seed = Rnd.Range(int.MinValue, int.MaxValue);
            Log("The seed is: " + _seed.ToString());
            _isSeedSet = true;
        }

        _Rnd = new System.Random(_seed);
        // SET SEED ABOVE IN CASE OF BUGS!!
        // _rnd = new System.Random(loggedSeed);
        _Module = Get<KMBombModule>();

        _Input.text = "";

        _Display.Assign(onInteract: () => { ChangeDisplay(); });
        _ClearButton.Assign(onInteract: () =>
        {
            ButtonEffect(_SubmitButton, 0.1f, KMSoundOverride.SoundEffect.ButtonPress);
            _Input.text = "";
        });
        _SubmitButton.Assign(onInteract: () => { Submit(); });

        for (int i = 0; i < _NumberButtons.Length; i++)
        {
            int x = i;
            _NumberButtons[i].Assign(onInteract: () => { NumberPress(x); });
        }

        _depth = _Rnd.Next(4, 9);
        _index = 0;
        do
        {
            _moduli = Enumerable.Range(2, 50).ToArray().Shuffle().Take(_depth).ToList();
            _lowestCommonMultiple = _moduli.Aggregate(1, (a, b) => LowestCommonMultiple(a, b));
        }
        while (_lowestCommonMultiple < 100 || int.MaxValue < _lowestCommonMultiple);

        _secret = _Rnd.Next(_moduli.ToArray().Max(), _lowestCommonMultiple);

        foreach(int moduli in _moduli)
            _remainder.Add(_secret % moduli);

        Log("────────<GENERATION>────────");

        for(int i = 0; i < _moduli.LengthOrDefault(); i++)
            Log("The number, modulo " + _moduli.ToArray()[i] + " is equal to " + _remainder.ToArray()[i]);

        Log("Therefore, a possible solution is: " + _secret.ToString());

        _OutputDisplay.text = "N % " + _moduli.ToArray()[_index] + " = " + _remainder.ToArray()[_index];
    }

    private int LowestCommonMultiple(int a, int b)
    {
        return a * b / GreatestCommonDivisor(a, b);
    }

    private int GreatestCommonDivisor(int a, int b)
    {
        if (b == 0)
            return a;
        else
            return GreatestCommonDivisor(b, a % b);
    }

    private void ChangeDisplay()
    {
        if (_isModuleSolved)
            return;
        ButtonEffect(_Display, 0.1f, KMSoundOverride.SoundEffect.ButtonPress);
        _index = (_index + 1) % _moduli.LengthOrDefault();
        _Index.text = (_index + 1).ToString();
        _OutputDisplay.text = "N % " + _moduli.ToArray()[_index] + " = " + _remainder.ToArray()[_index];
    }

    private void NumberPress(int index)
    {
        if (_isModuleSolved)
            return;
        ButtonEffect(_NumberButtons[index], 0.1f, KMSoundOverride.SoundEffect.ButtonPress);
        if (_Input.text.Length < 10)
            _Input.text += index.ToString();
    }

    private void Submit()
    {
        if (_isModuleSolved)
            return;
        ButtonEffect(_SubmitButton, 0.1f, KMSoundOverride.SoundEffect.ButtonPress);
        for (int i = 0; i < _moduli.LengthOrDefault(); i++)
            if (!(int.Parse(_Input.text) % _moduli.ToArray()[i] == _remainder.ToArray()[i]))
                _isAnswerCorrect = false;
        if(_isAnswerCorrect)
        {
            _isModuleSolved = true;
            Log("The correct number has been submitted. Module solved!");
            _OutputDisplay.text = "";
            _Index.text = "✓";
            _Module.HandlePass();
            PlaySound(KMSoundOverride.SoundEffect.CorrectChime);
        }
        else
        {
            Log("The number submitted, " + _Input.text + ", is incorrect. Strike and reset!");
            _Input.text = "";
            _moduli.Clear();
            _remainder.Clear();
            _Module.HandleStrike();
            _isAnswerCorrect = true;
            _isSeedSet = false;
            Start();
        }
    }
}
