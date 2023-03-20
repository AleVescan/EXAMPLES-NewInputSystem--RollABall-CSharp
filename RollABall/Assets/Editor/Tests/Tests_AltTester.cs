using System.Threading;
using AltTester.AltDriver;
using NUnit.Framework;
using UnityEngine;
using System;

public class Tests_AltTester
{
    public AltDriver altDriver;
    //Before any test it connects with the socket
    [OneTimeSetUp]
    public void SetUp()
    {
        altDriver = new AltDriver();
    }

    //At the end of the test closes the connection with the socket
    [OneTimeTearDown]
    public void TearDown()
    {
        altDriver.Stop();
    }

    [Test]
    public void TestMoveBallWithMoveMouse()
    {
        altDriver.LoadScene("MiniGame");
        var ball = altDriver.FindObject(By.NAME, "Player");
        var initialPosition = ball.GetWorldPosition();
        altDriver.MoveMouse(new AltVector2(0, 0), 3f);
        altDriver.MoveMouse(new AltVector2(100, 100), 3f);
        altDriver.MoveMouse(new AltVector2(-200, -200), 3f);
        ball = altDriver.FindObject(By.NAME, "Player");
        var finalPosition = ball.GetWorldPosition();
        Assert.AreNotEqual(initialPosition.x, finalPosition.x);
    }

    [Test]
    public void TestScrollOnScrollbar()
    {
        altDriver.LoadScene("MiniGame");
        var scrollbar = altDriver.FindObject(By.NAME, "Scrollbar Vertical");
        var scrollbarPosition = scrollbar.GetComponentProperty<float>("UnityEngine.UI.Scrollbar", "value", "UnityEngine.UI");
        altDriver.MoveMouse(altDriver.FindObject(By.NAME, "Scroll View").GetScreenPosition(), 1);
        altDriver.Scroll(new AltVector2(-3000, -3000), 1, true);
        var scrollbarPositionFinal = scrollbar.GetComponentProperty<float>("UnityEngine.UI.Scrollbar", "value", "UnityEngine.UI");
        Assert.AreNotEqual(scrollbarPosition, scrollbarPositionFinal);
    }

    [Test]
    public void TestMoveMouseOnScrollbar()
    {
        altDriver.LoadScene("MiniGame");
        var objects = altDriver.GetAllElementsLight();
        var scrollbar = altDriver.WaitForObject(By.NAME, "Handle");
        AltVector2 scrollbarInitialPosition = scrollbar.GetScreenPosition(); // use screen coordinates instead of world coordinates        
        altDriver.MoveMouse(scrollbar.GetScreenPosition()); // move mouse in area where scroll reacts
        altDriver.Scroll(-200, 0.1f);
        scrollbar = altDriver.WaitForObject(By.NAME, "Handle");
        AltVector2 scrollbarFinalPosition = scrollbar.GetScreenPosition();
        Assert.AreNotEqual(scrollbarInitialPosition.y, scrollbarFinalPosition.y);//compare y as there is no equality comparer on AltVector2. and we expect only y to change
    }

    [Test]
    public void TestSwipeOnScrollbar()
    {
        altDriver.LoadScene("MiniGame");
        var scrollbar = altDriver.WaitForObject(By.NAME, "Handle");
        AltVector2 scrollbarInitialPosition = new AltVector2(scrollbar.worldX, scrollbar.worldY);
        altDriver.Swipe(new AltVector2(scrollbar.x, scrollbar.y), new AltVector2(scrollbar.x, scrollbar.y - 200), 3);
        scrollbar = altDriver.WaitForObject(By.NAME, "Handle");
        AltVector2 scrollbarFinalPosition = new AltVector2(scrollbar.worldX, scrollbar.worldY);
        Assert.AreNotEqual(scrollbarInitialPosition.y, scrollbarFinalPosition.y);
    }

    [Test]
    public void TestClickNearScrollBarMovesScrollBar()
    {
        altDriver.LoadScene("MiniGame");

        var scrollbar = altDriver.WaitForObject(By.NAME, "Handle");
        var scrollbarInitialPosition = scrollbar.GetScreenPosition();

        var scrollBarMoved = new AltVector2(scrollbar.x, scrollbar.y - 100);
        altDriver.MoveMouse(scrollBarMoved, 1);

        altDriver.Click(new AltVector2(scrollbar.x, scrollbar.y - 100));

        scrollbar = altDriver.WaitForObject(By.NAME, "Handle");
        var scrollbarFinalPosition = scrollbar.GetScreenPosition();

        Assert.AreNotEqual(scrollbarInitialPosition.y, scrollbarFinalPosition.y);
    }

    [Test]
    public void TestBeginMoveEndTouchMovesScrollbar()
    {
        altDriver.LoadScene("MiniGame");
        var scrollbar = altDriver.FindObject(By.NAME, "Handle");
        var scrollbarPosition = scrollbar.GetScreenPosition();
        int fingerId = altDriver.BeginTouch(scrollbar.GetScreenPosition());
        altDriver.MoveTouch(fingerId, scrollbarPosition);
        AltVector2 newPosition = new AltVector2(scrollbar.x, scrollbar.y - 500);
        altDriver.MoveTouch(fingerId, newPosition);
        altDriver.EndTouch(fingerId);
        scrollbar = altDriver.FindObject(By.NAME, "Handle");
        var scrollbarPositionFinal = scrollbar.GetScreenPosition();

        Assert.AreNotEqual(scrollbarPosition.y, scrollbarPositionFinal.y);
    }

    [Test]
    public void TestPressKeyNearScrollBarMovesScrollBar()
    {
        altDriver.LoadScene("MiniGame");

        var scrollbar = altDriver.FindObject(By.NAME, "Handle");
        var scrollbarPosition = scrollbar.GetScreenPosition();
        var scrollBarMoved = new AltVector2(scrollbar.x, scrollbar.y - 100);
        altDriver.MoveMouse(scrollBarMoved, 1);
        altDriver.PressKey(AltKeyCode.Mouse0, 0.1f);
        scrollbar = altDriver.FindObject(By.NAME, "Handle");
        var scrollbarPositionFinal = scrollbar.GetScreenPosition();
        Assert.AreNotEqual(scrollbarPosition.y, scrollbarPositionFinal.y);
    }

    [Test]
    public void TestKeyDownAndKeyUpMouse0MovesScrollBar()
    {
        altDriver.LoadScene("MiniGame");

        var scrollbar = altDriver.FindObject(By.NAME, "Scrollbar Vertical");
        var handle = altDriver.FindObject(By.NAME, "Handle");
        var scrollbarPosition = scrollbar.GetComponentProperty<float>("UnityEngine.UI.Scrollbar", "value", "UnityEngine.UI");
        var scrollBarMoved = new AltVector2(handle.x, handle.y - 100);
        altDriver.MoveMouse(scrollBarMoved, 1);
        altDriver.KeyDown(AltKeyCode.Mouse0);
        altDriver.KeyUp(AltKeyCode.Mouse0);
        scrollbar = altDriver.FindObject(By.NAME, "Scrollbar Vertical");
        var scrollbarPositionFinal = scrollbar.GetComponentProperty<float>("UnityEngine.UI.Scrollbar", "value", "UnityEngine.UI");
        Assert.AreNotEqual(scrollbarPosition, scrollbarPositionFinal);
    }

    [Test]
    public void TestBallMovesOnPressKeys()
    {
        altDriver.LoadScene("MiniGame");

        var ball = altDriver.FindObject(By.NAME, "Player");
        altDriver.PressKey(AltKeyCode.S, 1f, 1f);
        var newBall = altDriver.FindObject(By.NAME, "Player");
        Debug.Log("Ball moved backward");
        Assert.AreNotEqual(ball.GetWorldPosition().z, newBall.GetWorldPosition().z);
        Thread.Sleep(1000);

        ball = altDriver.FindObject(By.NAME, "Player");
        altDriver.PressKey(AltKeyCode.W, 1f, 2f);
        newBall = altDriver.FindObject(By.NAME, "Player");
        Debug.Log("Ball moved forward");
        Assert.AreNotEqual(ball.GetWorldPosition().z, newBall.GetWorldPosition().z);
        Thread.Sleep(1000);

        ball = altDriver.FindObject(By.NAME, "Player");
        altDriver.PressKey(AltKeyCode.A, 1f, 2f);
        newBall = altDriver.FindObject(By.NAME, "Player");
        Debug.Log("Ball moved to the left");
        Assert.AreNotEqual(ball.GetWorldPosition().x, newBall.GetWorldPosition().x);
        Thread.Sleep(1000);

        ball = altDriver.FindObject(By.NAME, "Player");
        altDriver.PressKey(AltKeyCode.D, 1f, 2f);
        newBall = altDriver.FindObject(By.NAME, "Player");
        Debug.Log("Ball moved to the right");
        Assert.AreNotEqual(ball.GetWorldPosition().x, newBall.GetWorldPosition().x);
        Thread.Sleep(2000);
    }

    [Test]
    public void TestTiltBall()
    {
        altDriver.LoadScene("MiniGame");
        var ball = altDriver.FindObject(By.NAME, "Player");
        var initialPosition = ball.GetWorldPosition();
        altDriver.Tilt(new AltVector3(1000, 1000, 1), 3f);
        Assert.AreNotEqual(initialPosition.x, altDriver.FindObject(By.NAME, "Player").GetWorldPosition().x);
    }

    [Test]
    public void TestDoubleClick()
    {
        altDriver.LoadScene("MiniGame");
        var button = altDriver.FindObject(By.NAME, "SpecialButton").Click();
        Thread.Sleep(1000);
        button.Click();
        var text = altDriver.FindObject(By.PATH, "//ScrollCanvas/SpecialButton/Text (TMP)").GetText();
        Assert.AreEqual("2", text);
    }
    [Test]

    public void TestSameValueFourCountTextinDifferentWays()
    {
        altDriver.LoadScene("MiniGame");
        var ball = altDriver.FindObject(By.NAME, "Player");
        var initialPosition = ball.GetWorldPosition();
        var initialText = altDriver.FindObject(By.NAME, "CountText").GetText();


        const string componentName = "TMPro.TextMeshProUGUI";
        const string propertyName = "text";
        const string assemblyName = "Unity.TextMeshPro";
        var CountText = altDriver.FindObject(By.PATH, "/Canvas/CountText");
        var propertyValue = CountText.GetComponentProperty<string>(componentName, propertyName, assemblyName);
        Assert.AreEqual(initialText, propertyValue);

        //altDriver.MoveMouse(new AltVector2(0, 0), 1f);

        for (int counterKeys = 1; counterKeys < 2; counterKeys = counterKeys + 1)
        {

            altDriver.PressKey(AltKeyCode.S, 1f, 1f);
            Thread.Sleep(1000);
            altDriver.PressKey(AltKeyCode.W, 1f, 1f);
            Thread.Sleep(1000);
            altDriver.PressKey(AltKeyCode.A, 1f, 1f);
            Thread.Sleep(1000);
            altDriver.PressKey(AltKeyCode.D, 1f, 2f);
            Thread.Sleep(2000);
            altDriver.PressKey(AltKeyCode.S, 1f, 1f);

        }


        var finalText = altDriver.FindObject(By.NAME, "CountText").GetText();
        Assert.AreNotEqual(initialText, finalText);


    }

    [Test]

    public void TestCountNumberSameAsPickedupItems()
    {
        altDriver.LoadScene("MiniGame");
        var ball = altDriver.FindObject(By.NAME, "Player");
        var initialPosition = ball.GetWorldPosition();


        const string componentName = "UnityEngine.Transform";
        const string propertyName = "gameObject.activeSelf";
        const string assemblyName = "UnityEngine.CoreModule";


        for (int counterKeys = 1; counterKeys < 5; counterKeys = counterKeys + 1)
        {

            altDriver.PressKey(AltKeyCode.S, 1f, 1f);
            Thread.Sleep(1000);
            altDriver.PressKey(AltKeyCode.W, 1f, 1f);
            Thread.Sleep(1000);
            altDriver.PressKey(AltKeyCode.A, 1f, 1f);
            Thread.Sleep(1000);
            altDriver.PressKey(AltKeyCode.D, 1f, 2f);
            Thread.Sleep(2000);
            altDriver.PressKey(AltKeyCode.S, 1f, 1f);

        }

       // Thread.Sleep(2000);

        //get a list of all Pick-up objects 

        Debug.Log("I can see what i need ");

        var pickUps = altDriver.FindObjectsWhichContain(By.NAME, "PickUp", enabled:false);
        int countActive = 0;
        Assert.AreEqual(13, pickUps.Count);

        // count the items that have activeSelf property set to True
        foreach (var pickUp in pickUps)

        {
            Debug.Log("I was seen as a pickup");

            var flagActive = pickUp.GetComponentProperty<bool>(componentName, propertyName, assemblyName);
            if (flagActive == true)
            {
                Debug.Log("pickUp has active flag and is visible");
                countActive= countActive+1; 
            }
        }

            Debug.Log("Count Active is"+ countActive);

            var countCollected = 13 - countActive;

            var CountText = altDriver.FindObject(By.NAME, "CountText").GetText();
            Assert.AreEqual("Count: " + countCollected, CountText);
            // pickUps.Count will retrieve 13 because is also counts the PickupParent, which is not a pickUp ball that needs to be counted



        
    }

    [Test]

    public void TestCollectAllItems()
    {
        altDriver.LoadScene("MiniGame");
        var ball = altDriver.FindObject(By.NAME, "Player");
        var initialPosition = ball.GetWorldPosition();

         for (int i = 1; i <30; i = i + 1)
        {

            altDriver.PressKey(AltKeyCode.D, 1f, 0.9f);
            Thread.Sleep(75*i);
            altDriver.PressKey(AltKeyCode.S, 1f, 0.9f);
            Thread.Sleep(75*i);
            altDriver.PressKey(AltKeyCode.A, 1f, 0.9f);
            Thread.Sleep(75*i);
            altDriver.PressKey(AltKeyCode.W, 1f, 2f);
            Thread.Sleep(75*i);
        }

       
        var winText = altDriver.FindObject(By.NAME,"WinText");
        var CountText = altDriver.FindObject(By.NAME, "CountText").GetText();
        Assert.AreEqual(CountText, "Count: 12");
        Assert.NotNull(winText);

    }
}

