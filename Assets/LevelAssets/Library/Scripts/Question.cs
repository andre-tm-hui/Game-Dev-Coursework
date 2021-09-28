using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question : MonoBehaviour
{
    private int answerNum;

    public List<GameObject> questions = new List<GameObject>();
    private GameObject currentQuestion;
    private int questionNum = 0;

    private bool finished = false;
    public int maxQuestions = 5;

    // Start is called before the first frame update
    void Start()
    {
        changeQuestion();
    }

    public void changeQuestion()
    {
        Debug.Log("change");
        if (currentQuestion)
        {
            Debug.Log("destroy");
            Destroy(currentQuestion);
        }
        if (!finished)
        {
            Debug.Log("select");
            int i = Random.Range(0, questions.Count);
            currentQuestion = Instantiate(
                questions[i],
                transform.position + questions[i].transform.position,
                Quaternion.identity
                );
            currentQuestion.transform.parent = transform;
            answerNum = currentQuestion.GetComponent<Answer>().answer;
            questions.RemoveAt(i);
            questionNum++;
            if (questions.Count <= 0 || questionNum >= maxQuestions)
            {
                finished = true;
            }
        }
    }

    public bool isFinished()
    {
        return finished;
    }

    public int getAnswer()
    {
        return answerNum;
    }
}
