using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Logger;

namespace Semanticer.TextAnalyzer
{
    public static class EvaluatorHelper
    {
        public static bool Evaluate(IPostSematicEvaluator evaluator, IList<Post> postsList)
        {
            try
            {
                lock (evaluator)
                {
                    foreach (var post in postsList)
                    {
                        evaluator.Evaluate(post);
                    }
                }
                var positive = postsList.Where(x => x.MarkType == PostMarkType.Positive);
                var negative = postsList.Where(x => x.MarkType == PostMarkType.Negative);
                if (negative.Any() || positive.Any())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                LoggProvider.LoggError(0, "Semantic calculation: ", e);
            }
            if (postsList == null || postsList.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool Evaluate(IPostSematicEvaluator evaluator, IPostDataProvider provider)
        {
            List<Post> postsList = null;
            try
            {
                var posts = provider.GetPostToEvaluateSentiment(100);
                postsList = posts as List<Post> ?? posts.ToList();
                lock (evaluator)
                {
                    evaluator.Evaluate(postsList);
                }
                provider.SavePostsSemantic(postsList);
            }
            catch (Exception e)
            {
                LoggProvider.LoggError(0, "Semantic calculation: ", e);
            }
            if (postsList == null || postsList.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static IPostSematicEvaluator TrainablePostSematicEvaluator(ITextAnalizerDataProvider provider,
            LoggProvider logger, LearnigAlghoritm algorithm)
        {
            try
            {
                var evaluator = new TrainablePostSematicEvaluator(provider, logger, algorithm,
                   new SimplePivotWordProviderFactory(), new BigramTokenizerFactory());

                foreach (var langTrade in provider.TrainSupportedLanguages())
                {
                    try
                    {
                        evaluator.LoadClassifier(langTrade.Item1, langTrade.Item2);
                    }
                    catch (IOException e)
                    {
                        LoggProvider.LoggError(0, string.Format("{0}_{1}", langTrade.Item1, langTrade.Item2), e);
                        evaluator.TrainClassifier(false, langTrade.Item1, langTrade.Item2);
                    }
                }
                return evaluator;
            }
            catch (Exception e)
            {
                LoggProvider.LoggError(0, "Semantic calculation: ", e);
                return new SimplePostSemanticEvaluator(new NoteProvider(provider,new SimplePivotWordProviderFactory()));
            }
        }


    }
}