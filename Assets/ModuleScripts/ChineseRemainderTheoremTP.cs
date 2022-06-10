using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KeepCoding;

public class ChineseRemainderTheoremTP : TPScript<ChineseRemainderTheoremScript>
{

#pragma warning disable 414
    private string TwitchHelpMessage = "<!{0} submit 381654729> to submit 381654729 as your answer. <!{0} cycle> to cycle through all modular equations.";
#pragma warning restore 414

    public override IEnumerator ForceSolve()
    {
        string answer = Module._secret.ToString();
        for (int i = 0; i < answer.Length; i++)
        {
            yield return new WaitForSeconds(0.1f);
            yield return null;
            Module._NumberButtons[int.Parse(answer[i].ToString())].OnInteract();
        }
        yield return new WaitForSeconds(0.1f);
        yield return null;
        Module._SubmitButton.OnInteract();
    }

    public override IEnumerator Process(string command)
    {
        string[] split = command.ToUpperInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if(split[0] == "CYCLE")
        {
            for (int i = 0; i < Module._moduli.LengthOrDefault(); i++)
            {
                yield return new WaitForSeconds(1.5f);
                yield return null;
                Module._Display.OnInteract();
            }
        }
        else if (split[0] == "SUBMIT")
        {
            int submission = 0;
            if (split.Length != 2)
                yield return "sendtochaterror Expected another parameter for submission. Try again.";
            else if (!int.TryParse(split[1], out submission))
                yield return "sendtochaterror The string submitted is not a valid integer. Try again.";
            else
            {
                for (int i = 0; i < split[1].Length; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    yield return null;
                    Module._NumberButtons[int.Parse(split[1][i].ToString())].OnInteract();
                }
                yield return new WaitForSeconds(0.1f);
                yield return null;
                Module._SubmitButton.OnInteract();
            }
        }
    }
}
