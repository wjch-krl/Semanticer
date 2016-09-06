import gensim
import sys
from gensim import models
import itertools
from gensim import utils

sentences = models.doc2vec.TaggedLineDocument("corpa.txt")
model = models.Doc2Vec(alpha=0.025, min_alpha=0.025)  # use fixed learning rate
model.build_vocab(sentences)
for epoch in range(10):
    model.train(sentences)
    model.alpha -= 0.002  # decrease the learning rate
    model.min_alpha = model.alpha  # fix the learning rate, no decay

docvecs = list(model.docvecs)[:count]
for idx,element in enumerate(docvecs):
    print u'{0} {1}'.format(' '.join(sentences[idx].tags),' '.join(str(x) for x in element))