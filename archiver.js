import fetch from 'node-fetch';
import jsdom from 'jsdom';
import * as readLine from 'readline-sync';

let url = '';
let userChoice = '';
let id = '';
let invalidInput = true;
let domList = [];
let commentsIn = [];
let commentFrequency = [];
let postsIn = [];
let postFrequency = [];
let communities = [];
let hasNext = true;

const sleep = function(ms) {
    let time = new Date().getTime();
    while (time + ms >= new Date().getTime()) {

    }
}

const getArchive = async function() {
    console.log(url);
    while (hasNext) {
        // Define random wait value in milliseconds, between 1000ms
        // and 3000ms
        let randomWait = ((Math.random() * 3) + 1) * 1000;
        // Fetch HTML with URL; a string of all HTML
        // is returned and is parsed into a DOM object
        // with JSDOM.
        let dom = await fetch(url)
        .then((response) => {
          return response.text();
        })
        .then((html) => {
          let dom = new jsdom.JSDOM(html);
          return dom;
        });
        // Push page to domList
        domList.push(dom);
        let comments = dom.window.document.getElementsByClassName('thing comment');
        for (let element of Array.from(comments)) {
            commentsIn.push(element.querySelector('.subreddit').innerHTML);
        }
        let posts = dom.window.document.getElementsByClassName('thing link');
        for (let element of Array.from(posts)) {
            postsIn.push(element.querySelector('.subreddit').innerHTML);
        }
        // Automatically progress through each page returned by finding the 
        // next button, if it exists, and pulling the href value from the 
        // nested anchor tag. If there are no further results, there will
        // be no next button, and this may terminate.
        let nextButton = dom.window.document.getElementsByClassName('next-button').item(0);
        if (nextButton !== null) {
            url = nextButton.children.item(0).getAttribute('href');
            console.log(url);
            // sleep(randomWait);
        } else {
            hasNext = false;
        }
    }
    console.log(domList.length + ' pages of history parsed.');
}

const createLists = function() {
    console.log('Creating lists...');
    for (let community of commentsIn) {
        if (!commentFrequency.some((sub) => sub.subreddit == community)) {
            commentFrequency.push({subreddit: community, frequency: 1});
            communities.push(community);
            console.log('Adding ' + community + ' to comment frequency list with initial frequency of 1.');
        } else {
            commentFrequency[communities.indexOf(community)].frequency += 1;
            console.log('Updating ' + community + ' frequency to ' + commentFrequency[communities.indexOf(community)].frequency);
        }
    }
    communities = [];
    for (let community of postsIn) {
        if (!postFrequency.some((sub) => sub.subreddit == community)) {
            postFrequency.push({subreddit: community, frequency: 1});
            communities.push(community);
            console.log('Adding ' + community + ' to post frequency list with initial frequency of 1.');
        } else {
            postFrequency[communities.indexOf(community)].frequency += 1;
            console.log('Updating ' + community + ' frequency to ' + postFrequency[communities.indexOf(community)].frequency);
        }
    }
    for (let entry of commentFrequency) {
        console.log(entry);
    }
    for (let entry of postFrequency) {
        console.log(entry);
    }
}

while (invalidInput) {
    userChoice = readLine.question('Archive user (u) or subreddit (s), or quit (q)? Enter u or s or q: ');
    if (userChoice.toLocaleLowerCase() === 'u') {
        invalidInput = false;
        id = readLine.question('Enter username: ');
        url = 'https://old.reddit.com/user/' + id;
        getArchive().then(() => {createLists();});
    } else if (userChoice.toLocaleLowerCase() === 's') {
        invalidInput = false;
        id = readLine.question('Enter subreddit name: ');
        url = 'https://old.reddit.com/r/' + id;
        getArchive();
        createLists();
    } else if (userChoice.toLocaleLowerCase() === 'q') {
        console.log('Goodbye!')
        invalidInput = false;
    } else {
        console.log('Invalid input! Starting over...');
    }
}


