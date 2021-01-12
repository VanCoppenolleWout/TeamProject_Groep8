let showResult = (queryResponse) => {
    // log all objects
    for (const element of queryResponse) {
        console.log(element);
      }

    // Top 3 objects
    var nr1 = queryResponse[0];
    var nr2 = queryResponse[1];
    var nr3 = queryResponse[2];
    const top3 = document.querySelector(".o-layout__top3");
    top3.innerHTML = `<div class="o-layout__individueel">
                                <p class="c-card__name">${nr2.playername}</p>
                                <p class="c-card__score">${nr2.score}</p>
                                <div class="c-card__podium2"><p class="c-position">2</p></div>
                            </div>
                            <div class="o-layout__individueel">
                                <p class="c-card__name">${nr1.playername}</p>
                                <p class="c-card__score">${nr1.score}</p>
                                <div class="c-card__podium1"><p class="c-position">1</p></div>
                            </div>
                            <div class="o-layout__individueel">
                                <p class="c-card__name">${nr3.playername}</p>
                                <p class="c-card__score">${nr3.score}</p>
                                <div class="c-card__podium3"><p class="c-position">3</p></div>
                            </div>`;

    // List outside of top 3
    const html_list = document.querySelector(".js-leaderboard");
    var html_place = 3;
    var arrLeaderboard = queryResponse.slice(3);

    for (const element of arrLeaderboard) {
        html_place = html_place + 1;
        html_list.innerHTML += `<ul class="o-list o-layout__list">
                            <div class="o-layout__list-item">
                                <li class="c-list__position">${html_place}</li>
                                <li class="c-list__name">${element.playername}</li>
                            </div>
                            <li class="c-list__score">${element.score}</li>
                        </ul>`;
      };
};

const getAPI = async () => {
    const data = await fetch(`https://trappenspel-api.azurewebsites.net/api/leaderboard`)
        .then((r) => r.json())
        .catch((err) => console.error('An error occured', err));
        showResult(data);
    //console.log(data);
};

document.addEventListener('DOMContentLoaded', function() {
    getAPI();
});