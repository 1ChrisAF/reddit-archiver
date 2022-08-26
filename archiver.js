import fetch from 'node-fetch';
import jsdom from 'jsdom';
import * as readLine from 'readline-sync';
import { RawList } from './RawList.js';
import { FrequencyList } from './FrequencyList.js';

const getURL = function() {
    let url = '';
    let userChoice = '';
    let userName = '';
    let invalidInput = true;
    while (invalidInput) {
        console.log('ACTIONS:\n=============\nQUIT: Enter \'q\'\nARCHIVE USER: \'u\'\n');
        userChoice = readLine.question('CHOOSE: ');
        if (userChoice.toLocaleLowerCase() === 'u') {
            invalidInput = false;
            userName = readLine.question('Enter username: ');
            url = 'https://old.reddit.com/user/' + userName;
        } else if (userChoice.toLocaleLowerCase() === 'q') {
            console.log('Goodbye!')
            invalidInput = false;
            url = '';
        } else {
            console.log('Invalid input! Starting over...');
        }
    }
    return url;
}

const getArchive = async function(url, rawC, rawP) {
    console.log('Traversing user history...\n')
    console.log(url);
    let hasNext = true;
    let tracker = 1;
    while (hasNext) {
        tracker += 1;
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
        let comments = dom.window.document.getElementsByClassName('thing comment');
        for (let element of Array.from(comments)) {
            rawC.add(element.querySelector('.subreddit').innerHTML);
        }
        let posts = dom.window.document.getElementsByClassName('thing link');
        for (let element of Array.from(posts)) {
            rawP.add(element.querySelector('.subreddit').innerHTML);
        }
        // Automatically progress through each page returned by finding the 
        // next button, if it exists, and pulling the href value from the 
        // nested anchor tag. If there are no further results, there will
        // be no next button, and this may terminate.
        let nextButton = dom.window.document.getElementsByClassName('next-button').item(0);
        if (nextButton !== null) {
            url = nextButton.children.item(0).getAttribute('href');
            console.log(url);
        } else {
            hasNext = false;
        }
    }
    console.log(tracker + ' pages of history parsed.');
    return [rawC, rawP];
}

const createLists = function(rawC, rawP) {
    console.log('Creating lists...');
    let commentFrequencyList = rawC.toFrequencyList();
    let postFrequencyList = rawP.toFrequencyList();
    return [commentFrequencyList, postFrequencyList];
}

const main = async function() {
    let url = getURL();
    let rawCommentList = new RawList();
    let rawPostList = new RawList();
    if (url != '') {
        let results = await getArchive(url, rawCommentList, rawPostList);
        let resultLists = createLists(results[0], results[1]);
        let commentList = resultLists[0];
        let postList = resultLists[1];
        commentList.sortListByFrequency();
        postList.sortListByFrequency();
        console.log('COMMENTS:');
        console.log(commentList.to_String());
        console.log('POSTS:');
        console.log(postList.to_String());
    }
    
}

main();