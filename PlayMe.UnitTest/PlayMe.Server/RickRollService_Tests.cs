using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Server;
using PlayMe.Server.Providers;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server
{
    [TestFixture]
    public class RickRollService_Tests : TestBase<RickRollService>
    {
        private string user = "vickers";
        [Test]
        public void if_track_is_rick_rolled_RickRoll_returns_original_track_with_new_link()
        {
            //arrange
            const string trackLink = "Rickrolled";
            var track = new Track
                            {
                                Link = trackLink,
                                Name = "MyTrackName",
                                
                            };

            var dsMock = GetMock<IDataService<RickRoll>>();
            dsMock.Setup(m => m.GetAll()).Returns(new List<RickRoll>{new RickRoll{PlayMeObject = new Track{Link = "Rickrolled"}}}.AsQueryable());

            var musicProviderMock = GetMock<IMusicProvider>();
            musicProviderMock.Setup(m => m.GetTrack(It.IsAny<string>(), user)).Returns(new Track { Link = "RickRoll" });

            var musicProviderFactoryMock = GetMock<IMusicProviderFactory>();
            musicProviderFactoryMock.Setup(m => m.GetMusicProviderByIdentifier(It.IsAny<string>())).Returns(musicProviderMock.Object);
            
            //act
            var outTrack = ClassUnderTest.RickRoll(track, user);

            //assert
            Assert.That(outTrack,Is.Not.EqualTo(track));            
            Assert.That(outTrack.Link,Is.EqualTo("RickRoll"));
            Assert.That(outTrack.Name, Is.EqualTo("MyTrackName"));
        }

        [Test]
        public void if_album_is_rick_rolled_RickRoll_returns_original_track_with_new_link()
        {
            //arrange
            const string trackLink = "MyTrack";
            const string trackName = "MyTrackName";
            const string albumLink = "RickRolled";
            const string albumName = "RickRolledName";
            var track = new Track
            {
                Link = trackLink,
                Name = trackName,
                Album = new Album{Link=albumLink, Name = albumName}
            };

            var dsMock = GetMock<IDataService<RickRoll>>();
            dsMock.Setup(m => m.GetAll()).Returns(new List<RickRoll> { new RickRoll { PlayMeObject = new PlayMeObject{ Link = albumLink } } }.AsQueryable());

            var musicProviderMock = GetMock<IMusicProvider>();
            musicProviderMock.Setup(m => m.GetTrack(It.IsAny<string>(), user)).Returns(new Track { Link = "RickRoll" });

            var musicProviderFactoryMock = GetMock<IMusicProviderFactory>();
            musicProviderFactoryMock.Setup(m => m.GetMusicProviderByIdentifier(It.IsAny<string>())).Returns(musicProviderMock.Object);

            //act
            var outTrack = ClassUnderTest.RickRoll(track, user);

            //assert
            Assert.That(outTrack, Is.Not.EqualTo(track));
            Assert.That(outTrack.Link, Is.EqualTo("RickRoll"));
            Assert.That(outTrack.Album.Link, Is.EqualTo(albumLink));
            Assert.That(outTrack.Album.Name, Is.EqualTo(albumName));

        }

        [Test]
        public void if_artist_is_rick_rolled_RickRoll_returns_original_track_with_new_link()
        {
            //arrange
            const string trackLink = "MyTrack";
            const string trackName = "MyTrackName";
            const string artistLink = "RickRolled";
            const string artistName = "RickRolledName";
            var track = new Track
            {
                Link = trackLink,
                Name = trackName,
                Artists = new[] { new Artist { Link = "AnArtist", Name = "ArtistName"}, new Artist { Link = artistLink, Name = artistName } }
            };

            var dsMock = GetMock<IDataService<RickRoll>>();
            dsMock.Setup(m => m.GetAll()).Returns(new List<RickRoll> { new RickRoll { PlayMeObject = new PlayMeObject{ Link = artistLink } } }.AsQueryable());

            var musicProviderMock = GetMock<IMusicProvider>();
            musicProviderMock.Setup(m => m.GetTrack(It.IsAny<string>(), user)).Returns(new Track { Link = "RickRoll" });

            var musicProviderFactoryMock = GetMock<IMusicProviderFactory>();
            musicProviderFactoryMock.Setup(m => m.GetMusicProviderByIdentifier(It.IsAny<string>())).Returns(musicProviderMock.Object);

            //act
            var outTrack = ClassUnderTest.RickRoll(track, user);

            //assert
            Assert.That(outTrack, Is.Not.EqualTo(track));
            Assert.That(outTrack.Link, Is.EqualTo("RickRoll"));
            Assert.That(outTrack.Artists.Last().Link, Is.EqualTo(artistLink));
            Assert.That(outTrack.Artists.Last().Name, Is.EqualTo(artistName));
        }

        [Test]
        public void if_artist_or_album_or_track_is_not_rick_rolled_RickRoll_returns_original_track()
        {
            //arrange
            var track = new Track
            {
                Link = "MyTrack",
                Name = "MyTrackName",
                Album = new Album{Link = "MyAlbum",Name = "MyAlbumName"},
                Artists = new[]{new Artist{Link="MyArtist",Name = "MyArtistName"}}
            };

            var dsMock = GetMock<IDataService<RickRoll>>();
            dsMock.Setup(m => m.GetAll()).Returns(new[]{ new RickRoll{ PlayMeObject = new PlayMeObject{ Link = "ATrack" } } }.AsQueryable());
            //act
            ClassUnderTest.RickRoll(track, user);

            //assert
            Assert.That(track.Link, Is.Not.Null);
            Assert.That(track.Link, Is.EqualTo("MyTrack"));
            Assert.That(track.Name, Is.EqualTo("MyTrackName"));
        }
    }
}
