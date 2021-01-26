let showResult = (queryResponse) => {
    // log all objects
    // for (const element of queryResponse) {
    //     console.log(element);
    //   }

    // Top 3 objects

    var nr1 = queryResponse[0];
    var nr2 = queryResponse[1];
    var nr3 = queryResponse[2];
    const top3 = document.querySelector(".o-layout__top3");

    if (queryResponse.length < 3) {        
        if (queryResponse[1] == null && queryResponse[2] == null && queryResponse[0] != null) {
            top3.innerHTML = `<div class="o-layout__individueel">
                        <p class="c-card__name"></p>
                        <p class="c-card__score"></p>
                        <div class="c-card__podium2"><p class="c-position">2</p></div>
                    </div>
                    <div class="o-layout__individueel js-listitem" data-nickname="${nr1.playername}">
                        <p class="c-card__name">${nr1.playername}</p>
                        <p class="c-card__score">${nr1.score}</p>
                        <div class="c-card__podium1"><p class="c-position">1</p></div>
                    </div>
                    <div class="o-layout__individueel">
                        <p class="c-card__name"></p>
                        <p class="c-card__score"></p>
                        <div class="c-card__podium3"><p class="c-position">3</p></div>
                    </div>`;
        }

        else if (queryResponse[2] == null && queryResponse[0] != null && queryResponse[0] != null) {
            top3.innerHTML = `<div class="o-layout__individueel js-listitem" data-nickname="${nr2.playername}">
                        <p class="c-card__name">${nr2.playername}</p>
                        <p class="c-card__score">${nr2.score}</p>
                        <div class="c-card__podium2"><p class="c-position">2</p></div>
                    </div>
                    <div class="o-layout__individueel js-listitem" data-nickname="${nr1.playername}">
                        <p class="c-card__name">${nr1.playername}</p>
                        <p class="c-card__score">${nr1.score}</p>
                        <div class="c-card__podium1"><p class="c-position">1</p></div>
                    </div>
                    <div class="o-layout__individueel">
                        <p class="c-card__name"></p>
                        <p class="c-card__score"></p>
                        <div class="c-card__podium3"><p class="c-position">3</p></div>
                    </div>`;
        }

        else if (queryResponse[1] == null && queryResponse[2] == null && queryResponse[0] == null) {
            top3.innerHTML = `<div class="o-layout__individueel">
                        <p class="c-card__name"></p>
                        <p class="c-card__score"></p>
                        <div class="c-card__podium2"><p class="c-position">2</p></div>
                    </div>
                    <div class="o-layout__individueel">
                        <p class="c-card__name"></p>
                        <p class="c-card__score"></p>
                        <div class="c-card__podium1"><p class="c-position">1</p></div>
                    </div>
                    <div class="o-layout__individueel">
                        <p class="c-card__name"></p>
                        <p class="c-card__score"></p>
                        <div class="c-card__podium3"><p class="c-position">3</p></div>
                    </div>`;
        }
        
    }
    else {
        top3.innerHTML = `<div class="o-layout__individueel js-listitem" data-nickname="${nr2.playername}">
                        <p class="c-card__name">${nr2.playername}</p>
                        <p class="c-card__score">${nr2.score}</p>
                        <div class="c-card__podium2"><p class="c-position">2</p></div>
                    </div>
                    <div class="o-layout__individueel js-listitem" data-nickname="${nr1.playername}">
                        <p class="c-card__name">${nr1.playername}</p>
                        <p class="c-card__score">${nr1.score}</p>
                        <div class="c-card__podium1"><p class="c-position">1</p></div>
                    </div>
                    <div class="o-layout__individueel js-listitem" data-nickname="${nr3.playername}">
                        <p class="c-card__name">${nr3.playername}</p>
                        <p class="c-card__score">${nr3.score}</p>
                        <div class="c-card__podium3"><p class="c-position">3</p></div>
                    </div>`;
    }

    // List outside of top 3
    const html_list = document.querySelector(".js-leaderboard");
    html_list.innerHTML = ``;
    var html_place = 3;
    var arrLeaderboard = queryResponse.slice(3);

    if (arrLeaderboard.length == 0) {
        html_list.innerHTML = ``;
    }

    for (const element of arrLeaderboard) {
        html_place = html_place + 1;
        html_list.innerHTML += `<ul class="o-list o-layout__list js-listitem js-name-data" data-nickname="${element.playername}" data-playerid="${element.playerID}">
                            <div class="o-layout__list-item">
                                <li class="c-list__position">${html_place}</li>
                                <li class="c-list__name">${element.playername}</li>
                            </div>
                            <div class="o-layout__list-item">
                                <li class="c-leaderboard__steps">${element.steps} steps</li>
                                <li class="c-list__score">${element.score}</li>
                            </div>
                        </ul>`;
    };
    
    document.querySelectorAll('.js-listitem').forEach(item => {
        item.addEventListener('click', event => {
            console.log(item.dataset.nickname);
            getAPIpersonal(item.dataset.nickname);
            toggleState();
        });
    });      
};

let showPersonalResult = (queryResponse) => {
    const html_list = document.querySelector(".js-personalleaderboard");
    const html_leaderboardname = document.querySelector(".js-leaderboard__title");
    html_list.innerHTML = ``;
    var html_place = 0;

    var top3 = queryResponse.slice(0, 3);
    
    for (const element of queryResponse) {
        html_place = html_place + 1;
        html_leaderboardname.innerHTML = `Prestaties <b>${element.playername}</b>`;
        var fulldate = element.date.slice(0, 16).replace(/-/g, "/").replace("T", " ");
        var actualdate = fulldate.slice(0, 10);
        var actualtime = fulldate.slice(11);
        html_list.innerHTML += `<ul class="o-list o-layout__list js-listitem js-name-data" data-nickname="${element.playername}" data-position="${html_place}">
                            <div class="o-layout__list-item">
                                <li class="c-list__position">${html_place}</li>
                                <li class="c-list__date"><strong style="font-weight: 500">${actualdate}</strong> ${actualtime}</li>
                            </div>
                            <div class="o-layout__list-item">
                                <li class="c-leaderboard__steps">${element.steps} steps</li>
                                <li class="c-list__score">${element.score}</li>
                            </div>
                        </ul>`;
    };
        var goud = document.querySelector('[data-position="1"]');
        if (goud) {
            goud.setAttribute('class', 'o-list o-layout__list js-listitem js-name-data c-list-item__gold');
        };

        var zilver = document.querySelector('[data-position="2"]');
        if (zilver) {
            zilver.setAttribute('class', 'o-list o-layout__list js-listitem js-name-data c-list-item__silver');
        };
        
        var brons = document.querySelector('[data-position="3"]');
        if (brons) {
            brons.setAttribute('class', 'o-list o-layout__list js-listitem js-name-data c-list-item__bronze');
        }
};

let highlightItem = (queryResponse) => {    
    var highlight = document.querySelector(`[data-playerid="${queryResponse.playerID}"]`);
        if (highlight) {
            highlight.setAttribute('class', 'o-list o-layout__list js-listitem js-name-data c-list-item__active');
        };
};

const toggleState = function() {
    var list_closed = document.querySelector(".js-list__closed");
    var list_open = document.querySelector(".js-list__open");
    var micro = document.querySelector(".js-microinteraction");
    var leaderboard__subtitle = document.querySelector(".c-leaderboard__subtitle");
    console.log(leaderboard__subtitle);

    var btn_opnieuw = document.querySelector(".js-button-opnieuw");
    var btn_klassement = document.querySelector(".js-button-klassement");

    if( list_open.style.display == "none" )
    {
       list_closed.style.display = "none";
       list_open.style.display = "block";
       micro.style.display = "flex";
       btn_opnieuw.style.display = "block";
       btn_klassement.style.display = "none";
       console.log("hide subtitle");
       leaderboard__subtitle.style.display = "none";
    }
    else
    {
       list_closed.style.display = "block";
       list_open.style.display = "none";
       micro.style.display = "none";
       btn_opnieuw.style.display = "none";
       btn_klassement.style.display = "block";
       console.log("toon subtitle");
       leaderboard__subtitle.style.display = "block";
    }
 };

const getAPI = async () => {
    const data = await fetch(`https://trappenspel-api.azurewebsites.net/api/leaderboard`)
        .then((r) => r.json())
        .catch((err) => console.error('An error occured', err));
        showResult(data);
    //console.log(data);
};



const getAPIdifficulty = async (difficulty) => {
    const data = await fetch(`https://trappenspel-api.azurewebsites.net/api/leaderboard/${difficulty}`)
        .then((r) => r.json())
        .catch((err) => console.error('An error occured', err));
        showResult(data);
        getAPIlatestPersonal('Wout');
};

const getAPIpersonal = async (name) => {
    const data = await fetch(`https://trappenspel-api.azurewebsites.net/api/personalleaderboard/${name}`)
        .then((r) => r.json())
        .catch((err) => console.error('An error occured', err));
        showPersonalResult(data);
        console.log(data);
};

const getAPIlatestPersonal = async (name) => {
    const data = await fetch(`https://trappenspel-api.azurewebsites.net/api/getlatestscore/${name}`)
        .then((r) => r.json())
        .catch((err) => console.error('An error occured', err));
        highlightItem(data);
        console.log(data);
};

document.addEventListener('DOMContentLoaded', function() {
    getAPI();
    getAPIlatestPersonal('Wout');

    const btn_makkelijk = document.querySelector(".js-filter__makkelijk");
    btn_makkelijk.addEventListener('click', event => {
        getAPIdifficulty("easy");
    });

    const btn_gemiddeld = document.querySelector(".js-filter__gemiddeld");
    btn_gemiddeld.addEventListener('click', event => {
        getAPIdifficulty("medium");
    });

    const btn_moeilijk = document.querySelector(".js-filter__moeilijk");
    btn_moeilijk.addEventListener('click', event => {
        getAPIdifficulty("hard");
    });

    const btn_opnieuw = document.querySelector(".js-button-klassement");
    btn_opnieuw.addEventListener('click', event => {
        location.reload();
        //toggleState(); 
    });
});