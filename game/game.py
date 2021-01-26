import paho.mqtt.client as mqtt
import json
import multiprocessing
import ctypes
import random
import time
import requests


def on_connect(client, userdata, flags, rc):
    global prefix

    client.subscribe(f"{prefix}quantitysteps")
    client.subscribe(f"{prefix}gamestart")
    client.subscribe(f"{prefix}gamestop")
    client.subscribe(f"{prefix}score")
    client.subscribe(f"{prefix}gamestarted")


def on_message(client, userdata, msg):
    global prefix, steps, ledsorder, beginCountdown, game, name, difficulty, score, step_quantity, seconds, doOnce, prevStepped, sleepTime, googleid

    if msg.topic == f"{prefix}quantitysteps":
        payload = json.loads(msg.payload)
        
        if game.value != True:
            if int(payload["steps"]) < 1:
                message = {"answer": "Step quantity must be higher than 0"}
                client.publish(f"{prefix}quantitysteps/answer", json.dumps(message))
            elif int(payload["steps"]) % 2 != 0:
                message = {"answer": "Step quantity must be divisible by 2"}
                client.publish(f"{prefix}quantitysteps/answer", json.dumps(message))
            else:
                steps.value = int(payload["steps"])
                print("Step quantity has been set to " + str(steps.value))

                message = "quantitysetoff"
                for i in range(1, 10):
                    client.publish(f"{prefix}step{i}", message)
                
                message = "quantityset"
                for i in range(0, int(steps.value)):
                    client.subscribe(f"{prefix}step{i + 1}/answer")
                    client.publish(f"{prefix}step{i + 1}", message)

                message = {"answer": "Steps has successfully been configured"}
                client.publish(f"{prefix}quantitysteps/answer", json.dumps(message))
    elif msg.topic == f"{prefix}gamestart":
        score = 0
        step_quantity = False
        payload = json.loads(msg.payload)

        if game.value != True:
            if payload["name"] != "" and (payload["difficulty"] == "easy" or payload["difficulty"] == "normal" or payload["difficulty"] == "hard"):
                print(payload)
                beginCountdown.value = True;
                doOnce = True
                name = payload["name"]
                difficulty = payload["difficulty"]
                googleid = payload["googleid"]

                ledsorder[:] = []
                ledsorder.append(1)
                for i in range(1, steps.value):
                    ledsorder.append(3)
                
                if difficulty == "easy":
                    sleepTime.value = 5
                elif difficulty == "normal":
                    sleepTime.value = 4
                elif difficulty == "hard":
                    sleepTime.value = 2
                
                message = {"gamestarted": game.value}
                client.publish(f"{prefix}gamestarted/answer", json.dumps(message))

                print("Game has started", beginCountdown.value, name, difficulty)
                message = {"answer": "Game has started"}
                client.publish(f"{prefix}gamestart/answer", json.dumps(message))

                message = "quantitysetoff"
                for i in range(1, 10):
                    client.publish(f"{prefix}step{i}", message)
                
                message = "gamestart"
                for i in range(1, int(steps.value) + 1):
                    client.publish(f"{prefix}step{i}", message)
            else:
                message = {"answer": "Input is not correct"}
                client.publish(f"{prefix}gamestart/answer", json.dumps(message))
    elif msg.topic == f"{prefix}gamestop":
        stop_game(client)
    elif msg.topic == f"{prefix}score":
        score = int(msg.payload)
    elif msg.topic == f"{prefix}gamestarted":
        message = {"gamestarted": game.value}
        client.publish(f"{prefix}gamestarted/answer", json.dumps(message))
        print(f"Game started: {game.value}")
    
    stepStepped = -1
    ledsorderWhenSteppedOn = []

    for i in range(0, int(steps.value)):
        if msg.topic == f"{prefix}step{i + 1}/answer":
            feet.value = i
            newStep.value = False
            stepStepped = i
            ledsorderWhenSteppedOn = ledsorder
    
    if stepStepped != -1 and prevStepped != stepStepped:
        if ledsorderWhenSteppedOn[stepStepped] == 1:
            score += 1
        elif ledsorderWhenSteppedOn[stepStepped] == 2:
            score += 2
    
    if stepStepped != -1 and ledsorderWhenSteppedOn[stepStepped] == 3:
        stop_game(client)
    
    prevStepped = stepStepped


def stop_game(client):
    global prefix, steps, ledsorder, beginCountdown, game, name, difficulty, score, step_quantity, seconds, doOnce, googleid

    if game.value == True:
        game.value = False
        beginCountdown.value = False

        print("Game stopped")
        message = {"game": game.value, "name": name, "score": score, "seconds": seconds}
        client.publish(f"{prefix}game/answer", json.dumps(message))

        url = "https://trappenspel-api.azurewebsites.net/api/postleaderboard"
        data = {
            "playername": name,
            "score": score,
            "difficulty": difficulty,
            "steps": steps.value,
            "googleid": googleid
        }

        resp = requests.post(url, json=data)

        message = "gamestop"
        for i in range(1, int(steps.value) + 1):
            client.publish(f"{prefix}step{i}", message)
        
        steps.value = 0
        difficulty = ""
        name = ""
        seconds = 5
        googleid = ""


def led_color_timer(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore, sleepTime):
    client = mqtt.Client()
    client.connect("13.81.105.139", 1883, 60)
    client.on_connect = on_connect
    client.on_message = on_message

    localFeet = feet.value

    if ledsorder[int(localFeet)] != 3:
        ledsorder[int(localFeet)] = 2
        time.sleep(sleepTime.value)
        ledsorder[int(localFeet)] = 3

    # print(ledsorder)


def led_loop(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore):
    client = mqtt.Client()
    client.connect("13.81.105.139", 1883, 60)
    client.on_connect = on_connect
    client.on_message = on_message

    while True:
        if game.value:
            for i in range(0, int(steps.value)):
                client.publish(f"{prefix}step{i + 1}", ledsorder[i])
                time.sleep(0.5)
            
            print(ledsorder)


def sensor_loop(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore):
    client = mqtt.Client()
    client.connect("13.81.105.139", 1883, 60)
    client.on_connect = on_connect
    client.on_message = on_message

    while True:
        if game.value:
            for i in range(0, int(steps.value)):
                client.publish(f"{prefix}step{i + 1}/sensor", "1")
                time.sleep(0.01)


def main_loop(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore, sleepTime):
    client = mqtt.Client()
    client.connect("13.81.105.139", 1883, 60)
    client.on_connect = on_connect
    client.on_message = on_message
    
    while True:
        if game.value and newStep.value == False:
            if steps.value > 8:
                maxLights = 6
            elif steps.value >= 6:
                maxLights = 2
            else:
                maxLights = 2
            
            for i in range(0, steps.value):
                if i == int(feet.value):
                    # ledsorder[int(feet.value)] = 2
                    led_color_timer_process = multiprocessing.Process(target=led_color_timer, args=(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore, sleepTime))
                    led_color_timer_process.start()
                elif lightUpLeds.value <= maxLights and ledsorder[i] != 2:
                        rand = random.randint(0, 1000)
                        if rand < 200:
                            ledsorder[i] = 1
                            lightUpLeds.value += 1
            
            begin = int(feet.value) - 2
            end = int(feet.value) + 2

            if begin < 0:
                begin = 0
            
            if end >= steps.value - 1:
                end = steps.value - 1
            else:
                end += 1
            
            hasGreen = False
            for i in range (begin, end):
                if ledsorder[i] == 1:
                    hasGreen = True
            
            if hasGreen != True:
                if feet.value == 9:
                    ledsorder[begin] = 1
                else:
                    ledsorder[end] = 1
            
            lightUpLeds.value = 0
            for i in range (0, steps.value):
                if ledsorder[i] == 1:
                    lightUpLeds.value += 1

            # print(ledsorder)
            
            newStep.value = True

            for i in range(0, int(steps.value)):
                client.publish(f"{prefix}step{i + 1}", ledsorder[i])
                time.sleep(0.001)


if __name__ == "__main__":
    client = mqtt.Client()
    client.connect("13.81.105.139", 1883, 60)
    client.on_connect = on_connect
    client.on_message = on_message
    client.loop_start()

    prefix = "kobemarchal/groep8/"
    beginCountdown = multiprocessing.Value(ctypes.c_bool, False)
    game = multiprocessing.Value(ctypes.c_bool, False)
    doOnce = False
    name = ""
    difficulty = ""
    score = 0
    step_quantity = False
    seconds = 5
    prevStepped = -1
    googleid = ""

    manager = multiprocessing.Manager()

    sleepTime = multiprocessing.Value("i", 0)
    feet = multiprocessing.Value("i", 0)
    steps = multiprocessing.Value("i", 10)
    start = multiprocessing.Value(ctypes.c_bool, False)
    lightUpLeds = multiprocessing.Value("i", 0)
    newStep = multiprocessing.Value(ctypes.c_bool, True)
    stepColorBefore = multiprocessing.Value("i", 0)
    # 1 = green, 2 = orange, 3 = red
    ledsorder = manager.list()
    ledsorder.append(1)
    for i in range(1, steps.value):
        ledsorder.append(3)
    
    main_loop_process = multiprocessing.Process(target=main_loop, args=(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore, sleepTime))
    main_loop_process.start()
    
    led_loop_process = multiprocessing.Process(target=led_loop, args=(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore))
    led_loop_process.start()

    sensor_loop_process = multiprocessing.Process(target=sensor_loop, args=(feet, steps, start, lightUpLeds, newStep, ledsorder, prefix, game, stepColorBefore))
    sensor_loop_process.start()
    
    while True:
        if beginCountdown.value:
            if seconds > 0:
                seconds -= 1
                message = {"game": beginCountdown.value, "name": name, "score": score, "seconds": seconds}
                print(f"Countdown: {seconds}")
                client.publish(f"{prefix}game/answer", json.dumps(message))
                time.sleep(1)

            if seconds == 0:
                message = {"game": game.value, "name": name, "score": score, "seconds": 0}
                client.publish(f"{prefix}game/answer", json.dumps(message))
                if doOnce:
                    game.value = True
                    doOnce = False

                    for i in range(0, int(steps.value)):
                        client.publish(f"{prefix}step{i + 1}", ledsorder[i])
                        time.sleep(0.001)
                time.sleep(0.2)