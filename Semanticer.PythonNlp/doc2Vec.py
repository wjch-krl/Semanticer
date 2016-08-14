import gensim
import sys
from gensim import models
import itertools
from gensim import utils

class DoubleTaggedLineDocument(object):

    def __init__(self, corpaFile, messagesFile):
        self.source = corpaFile
        self.messageReader = MessageReader(messagesFile)

    def processFile(self):
        try:
            self.source.seek(0)
            for item_no, line in enumerate(self.source):
                yield  models.doc2vec.TaggedDocument(utils.to_unicode(line).split(), [item_no])
        except AttributeError:
            with utils.smart_open(self.source) as fin:
                for item_no, line in enumerate(fin):
                    yield  models.doc2vec.TaggedDocument(utils.to_unicode(line).split(), [item_no])

    def __iter__(self):
        return itertools.chain(self.messageReader.processFile(),self.processFile())

class MessageReader():

    def __init__(self, messageFile):
        self.messageFile = messageFile

    def method_name(self, line):
        splitted = utils.to_unicode(line).split()
        return  models.doc2vec.TaggedDocument(splitted[3:], splitted[:3])

    def processFile(self):
        try:
            self.source.seek(0)
            for item_no, line in enumerate(self.messageFile):
                yield self.method_name(line)
        except AttributeError:
            with utils.smart_open(self.messageFile) as fin:
                for item_no, line in enumerate(fin):
                    yield self.method_name(line)

    def __iter__(self):
        return self.processFile()


wordVecSize = int(sys.argv[3])
sentences = DoubleTaggedLineDocument(sys.argv[1],sys.argv[2])
messages = list(sentences.messageReader)
count = len(messages)
model = models.doc2vec.Doc2Vec(sentences,size = wordVecSize)
docvecs = list(model.docvecs)[:count]
for idx,element in enumerate(docvecs):
    print u'{0} {1}'.format(' '.join(messages[idx].tags),' '.join(str(x) for x in element))