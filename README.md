#Data Pipeline Guidance
Microsoft patterns & practices

This reference implementation is a work-in-progress project. It is meant to demonstrate proven practices regarding the high-scale, high-volume ingestion of data in a typical event processing system.

The project makes heavy use of [Microsoft Azure Event Hubs](http://azure.microsoft.com/en-us/services/event-hubs/), a cloud-scale telemetry ingestion service. Familiarity with the [general concepts underlying Event Hubs](http://msdn.microsoft.com/en-us/library/azure/dn789972.aspx) is very useful for understanding the source in this reference implementation.
True story: The first Super Bowl was such a novelty America was watching a commercial during the actual kickoff. The TV producer told sideline reporter Pat Summerall to ask Green Bay coach Vince Lombardi to kick over again.

Summerall didn't. But that's not the point, which is you won't miss anything involving the Super Bowl for the next two weeks. Everything's big. Even the commercials. Especially them.

As a cheater's guide for those who aren't football-fluent or simply want to dodge the coming tsunami of Super Bowl coverage, here's the Top 10 themes for the game between Seattle and New England:

-1. "U MAD BRO?" This will be the go-to meme of the Super Bowl. Seattle cornerback Richard Sherman tweeted these words on a photo of himself jawing at New England quarterback Tom Brady after their 2012 game. Sherman yelled at Brady all game with lines like, "Please keep trying me, I'm going to take it from you." This, of course, was the first time anyone heard of Sherman's impressive mouth. It's been running non-stop since then. Can Brady puh-leeze shut this man up?

-2. Pete Carroll preceded Bill Belichick in New England. Like Picasso had his, "Blue Period," Patriots owner Robert Kraft had a "Jerry Jones Period." Kraft had just bought the team, feuded with Bill Parcells (what owner didn't?) and hired Carroll in 1997. Kraft wore a stopwatch to practices in those days, talked about drafting some, "press corners," and constantly interfered with Carroll. The Pats went 27-21 in Carroll's three years. Kraft then hired Belichick and has been in his "Genius Owner Period" ever since.

-3. Marshawn Lynch Sound Bites. Like a 4-year-old who says something funny once, Lynch keeps thinking he's cute with repeated like, "Thanks for asking," to any media question one week and, "I'm thankful" another. And the media, dumb as it (we) are, keep asking. The Super Bowl test will Lynch, though. He must come up with three straight days of answers. Here's my suggestions: "I'm blessed; "I'm here" and "Let's go."

-4. The Dolphins are close. OK, maybe this isn't a national theme. But this is an annual Super Bowl staple known as the Dave Wannstedt Memorial Line since the 2001 Super Bowl when then-Dolphins coach repeatedly told local media, "We're close." Just as they've repeatedly echoed most years since. And why not? They beat Seattle in their last meeting in 2012. They also beat New England in September this year and played them to a tough, 31-point loss in December. Hey, that's closer than Indianapolis played them Sunday.

-5. September doesn't matter. New England started 2-2 and idiots said the Patriots Dynasty had an expiration date (wait, was that me?). Seattle started 3-3 and the dreaded Super Bowl Hangover tag was hung. You know what month matters? January.

-6. Home field matters, too. Eight of the 10 playoffs wins came on the home field. That includes both of Sunday's championship games. In other playoffs like the NBA, the home-field edge is a slim one. But playing at home in the NFL means so much, especially in Seattle where the "12th Man" never gives up ... never gives in ... never ... uh ...

-7. Seattle's "12th Man" wasn't believing. The Super Bowl is in Phoenix, and by then maybe they'll be back. But hundreds of Seattle fans left Sunday's game early with their team down late. Sound familiar, Heat fans? And how long did America chuckle over your leaving Game 6 against San Antonio? The difference is South Florida knows it's a fair-weather town (or it should know). Seattle considers itself the gold standard of fandom. But "The 12th Man" shrunk to "The 11 1/2 Man?"

-8. Roger Goodell on display. This will be a pre-game highlight. If you throw out the trifling $44.2 million he earned in 2014, Goodell only lacked boils covering his body this year from being Old Testament troubling. There was Ray Rice and domestic abuse. There was Adrian Peterson and child abuse. And there was Goodell making the rules up as he went along. He'll get asked about it all during Super Bowl Week. Also hopefully this from a South Florida reporter: Do Seattle and New England need roofs if they want to host championship games again considering all that rain?

-9. Coffee versus Chowder. This says why you have to be cold-blooded, cold-hearted or downright un-American in the world's most over-caffeinated country to root against Seattle in this Super Bowl. Seriously, when's the last time you had a cup of chowder (all South Florida Dolphins media excluded considering it was in the New England press box in December). Coffee is an everyday experience. Sometimes twice a day, if you need to type fast on deadline. But just because Seattle should be everyone's hope doesn't mean ...

-10. New England 24, Seattle 17. Bill Belichick has two weeks to prepare his talented defense for a Seattle offense with no threatening receiver. The Patriots can win however the game bends on offense, either running or passing the ball. Yes, the cursed Patriots just won't stop taunting Dolphins fans. Can we hope for a commercial at the kickoff?

##Overview

The two primary concerns of this project are:

* Facilitating cold storage of data for later analytics. That is, translating the _chatty_ stream of events into _chunky_ blobs.

* Dispatching incoming events to specific handlers. That is, examining an incoming event and passing it along to an appropriate handler function. The emphasis of our dispatcher solution is on speed and overal throughput.

## Next Steps

* [Scenarios & Requirements](/docs/Introduction.md)
* [Architecture Overview](/docs/ArchitectureOverview.md)
* [Getting Started](/docs/GettingStarted.md)
