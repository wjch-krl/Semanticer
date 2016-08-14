using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;
using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    public class DatabaseTrainingData : ITrainingData
    {
        private readonly ITokenizer tokenizer;
        private readonly int langId;
        private readonly int tradeId;
        private readonly double trainProportion;

        public DatabaseTrainingData(ITextAnalizerDataProvider databaseProvider, bool loadWords, ITokenizer tokenizer,
            string lang, int tradeId, double trainProportion)
        {
            this.tokenizer = tokenizer;
            langId = databaseProvider.LangId(lang);
            this.tradeId = tradeId;
            this.trainProportion = trainProportion;
            DatabaseProvider = databaseProvider;
            LoadWords = loadWords;
        }

        public ITrainingEventReader Reader => new DatabaseTrainReader(DatabaseProvider, tokenizer, langId, tradeId, trainProportion);

        public ITextAnalizerDataProvider DatabaseProvider { get; private set; }

        public bool LoadWords { get; set; }
    }

    public interface IClientDataProvider
    {
        HashSet<string> GetStopList(int id);
        IDictionary<string, BlockedAuthor> FetchBlockedUsers(int profileId);
        void SaveBlockedUsers(IEnumerable<BlockedAuthor> authors);
        void SaveAuthorAsBlockedUser(Author auth, int profileId);
        void DeleteBlockedUsers(Profile profile, IDictionary<string, BlockedAuthor> skipUsersDb);
    }

    public interface ITextAnalizerDataProvider
    {
        Dictionary<int, string> FetchDictionary();
        IEnumerable<string> GetProperNames();
        IEnumerable<LexiconWord> NegativeWords();
        IEnumerable<LexiconWord> PositiveWords();
        IEnumerable<LexiconWord> AllWords(int lang, int tradeId);
        IEnumerable<LexiconWord> Phrases(int lang, int tradeId);
        IEnumerable<LexiconWord> Emoticons();
        IEnumerable<string> WordsMatchingPattern(string pattern);
        IEnumerable<string> WordsAndForms(int lang);
        IDictionary<string, int> LangugaesDictionary();
        IEnumerable<string> NormalizeWord(string word, int langId);
        IEnumerable<string> FetchSynonyms(string word);
        IEnumerable<TrackedPhrase> FetchSynonyms(IEnumerable<TrackedPhrase> word);
        IEnumerable<Tuple<string, double>> ExecuteFNote(string normalizeMessage, string lang);
        IEnumerable<Post> TrainMessages(int langId, int trdId);
        IEnumerable<Tuple<string, int>> TrainSupportedLanguages();
        IEnumerable<string> StopWords(int lang);
        IEnumerable<string> SemanticStopWords(int lang);
        int LangId(string language);
        void SavePostsAsTrainingData(IList<Post> postsList);
        string ConnectionString { get; }
    }

    public interface IMainDataProvider
    {
        List<Profile> GetProfiles();
        List<Profile> GetProfiles(int sourceId);
        Profile GetProfileById(int profileId);
        void DeleteProfile(int profileId);
        //TODO add sourceId
        Profile GetProfileByOrgId(string profileOrgId);
        void UpdateProfileInfo(Profile profile);
        void UpdateConcreteProfile(Profile profile);
        void SaveProfiles(IList<Profile> result);
        bool UpdateSkiplist(Profile profile, IClientDataProvider clientData, IPostDataProvider postData);

        List<Source> GetSources();
        int GetSourceIdForName(string name);

        IEnumerable<DiagnosticLogElement> FetchLogRuns();
        IEnumerable<DiagnosticLogElement> FetchLogDetails(int runId);
        IEnumerable<DiagnosticLogElement> FetchLogDetails();
        int CreateRun(DiagnosticLogElement log);

        void SaveLink(Link link, int profilieId);
        void UpdateLink(Link link);
        IEnumerable<Link> NotVisitiedLinks(int profileId);

        string BestModelPath(int langId, int tradeId);

        TagDictionary GetTagDict(int value);
        int? GetDefaultTagDictId(ForumScriptType type);

        void SaveSummaryPosts(IEnumerable<PostAggreagate<Tuple<int, int, int, double>>> summary, int profileId);
        void SavePostsByDates(IEnumerable<PostAggreagate<Tuple<int, int, int, int, int, int>>> summary);
        void SaveTopPostsByCommentsMark(IEnumerable<PostAggreagate<double>> summary);
        void SaveTopPostsByCommentsCount(IEnumerable<PostAggreagate<int>> summary);
        void SaveTopPostsByLikes(IEnumerable<PostAggreagate<int>> summary);
        void SaveTopAuthorsByPosLikes(IEnumerable<PostAggreagate<Tuple<int, int, string>>> summary);
        void SaveTopAuthorsByPosCount(IEnumerable<PostAggreagate<Tuple<int, string>>> summary);
        void SaveWordCloud(IEnumerable<PostAggreagate<Tuple<string, int, int>>> summary);
        IEnumerable<TrackedPhrase> TrackedWords(int profileId);
        void SaveTrackedWordsInTime(IEnumerable<PostAggreagate<Tuple<string, int, int>>> words);
        IEnumerable<Profile> ProfilesForAnalisis(int analisisId);
        void SaveTrackedWords(IEnumerable<TrackedPhrase> phrases, int analysisId);
        IEnumerable<Profile> GetProfiles(string sourceId);
        void AssignProfileToAnalisis(int analaisisId, Profile profile);
        int CreateNewAnalisis();
        IDbConnection AttachToAnalisUpdate(int analisisId, Action onAnalizeCompleted);
        void SignalAnalisUpdate(int analisisId);
        void ClearAggregatesForAnalisis(int analisisId);
        void UpdateTrackedWords(IEnumerable<TrackedPhrase> toSave);
        IEnumerable<int> AnalisisWithProfile(Profile profile);
        IEnumerable<Profile> ProfilesWithOrgId(string orgId);
        IEnumerable<AnalysisProfile> Analysises();
        void DeleteAnalis(int analId);
        void UpdateAnalysis(int analId);
        IDbConnection AttachToAnalisRefreshRequest(Action<string> onAnalizeCompleted);
        void UpdateAnalysisDefinintion(AnalysisProfile analysisProfile);
    }

    public interface IPostDataProvider : IDatabasePostProvider
    {
        IDictionary<string, Author> FetchUsers();
        Author GetAuthorForOrgidId(string orgId, int sourceId);
        void SaveUsers(IEnumerable<Author> authors);
        void SaveAuthor(Author auth);
        IDictionary<string, int> GetUsersDictionary();
        IDictionary<string, int> GetUsersDictionary(int sourceId);
        IDictionary<string, string> FetchUsersLangDictionary(int sourceId);
        void SyncAuthors(IDictionary<string, Author> resultAuthors);
        IList<Author> PrepareUsersToSave(IDictionary<string, Author> resultAuthors, int sourceId);
        Author UpsertAuthor(Author author);
        Author[] UpsertAuthors(IEnumerable<Author> author);
        void ClearProfiles(IEnumerable<int> profiles);
    }

    public interface IDatabasePostProvider
    {
        IDictionary<string, Post> FetchPosts(List<Author> authors, int profileId);
        List<Post> GetPosts();
        List<Post> GetPostsForProfile(int profileId);
        IEnumerable<Post> PostsQuery(string filter, bool joinAuthors = false);
        IEnumerable<Post> FullTextSearchPosts(params string[] keyWords);
        Post GetPostByOrgId(string orgId, int profileId);
        //DELETE ???
        IDictionary<string, int> GetPostsDictionary(int profileId);
        void SetDeleted(IEnumerable<string> postsToDelete, int sourceId);
        void SavePosts(IEnumerable<Post> posts);
        void UpdatePost(Post post);
        void UpdatePosts(IEnumerable<Post> posts);
        void SavePostsSemantic(IList<Post> postsList);
        void PreparePostsToSave(IEnumerable<Post> posts, Profile profile, int sourceId);
        IEnumerable<Post> GetPostToEvaluateSentiment(int count);
        IEnumerable<Post> AllProfilePosts(Profile profile);
        IEnumerable<Post> FullTextSearchPosts(int profileId, params string[] keyWords);
        IEnumerable<Post> FullTextSearchPosts(int profileId, params TrackedPhrase[] keyWords);
        void UpdateMaxStrong(int profileId);
    }
}