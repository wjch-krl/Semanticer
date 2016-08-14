import os
import pdb
import numpy as np
import argparse
from collections import Counter

def tokenize(sentence, grams):
    words = sentence.split()
    tokens = []
    for gram in grams:
        for i in range(len(words) - gram + 1):
            tokens += ["_*_".join(words[i:i+gram])]
    return tokens

def build_dict(f, grams):
    dic = Counter()
    for sentence in open(f).xreadlines():
        dic.update(tokenize(sentence, grams))
    return dic

def process_files(file_pos, file_neg, dic, r, outfn, grams):
    output = []
    for beg_line, f in zip(["1", "-1"], [file_pos, file_neg]):
        for l in open(f).xreadlines():
            tokens = tokenize(l, grams)
            indexes = []
            for t in tokens:
                try:
                    indexes += [dic[t]]
                except KeyError:
                    pass
            indexes = list(set(indexes))
            indexes.sort()
            line = [beg_line]
            for i in indexes:
                line += ["%i:%f" % (i + 1, r[i])]
            output += [" ".join(line)]
    output = "\n".join(output)
    f = open(outfn, "w")
    f.writelines(output)
    f.close()

def compute_ratio(poscounts, negcounts, neutcounts, alpha=1):
    alltokens = list(set(poscounts.keys() + negcounts.keys() + neutcounts.keys()))
    dic = dict((t, i) for i, t in enumerate(alltokens))
    d = len(dic)
    print "computing r..."
    p, q, z = np.ones(d) * alpha , np.ones(d) * alpha , np.ones(d) * alpha
    for t in alltokens:
        p[dic[t]] += poscounts[t]
        q[dic[t]] += negcounts[t]
        z[dic[t]] += neutcounts[t]
    p /= abs(p).sum()
    q /= abs(q).sum()
    z /= abs(z).sum()
    # TODO
    sum = p + q + z
    r = np.log(p/q)
    r1 = np.log(sum/p)
    r2 = np.log(sum/q)
    r3 = np.log(sum/z)
    return dic, r
 
def main(ptrain, ntrain, ptest, ntest, out, liblinear, ngram):
    ngram = [int(i) for i in ngram]
    print "counting..."
    poscounts = build_dict(ptrain, ngram)         
    negcounts = build_dict(ntrain, ngram)         
   # neutcounts = build_dict(neutrain, ngram)         
    neutcounts = negcounts

    dic, r = compute_ratio(poscounts, negcounts ,neutcounts)
    print "processing files..."
    process_files(ptrain, ntrain, dic, r, "train-nbsvm.txt", ngram)
    process_files(ptest, ntest, dic, r, "test-nbsvm.txt", ngram)
    
    trainsvm = os.path.join(liblinear, "train.exe") 
    predictsvm = os.path.join(liblinear, "predict.exe") 
    os.system(trainsvm + " -s 0 train-nbsvm.txt model.logreg")
    os.system(predictsvm + " -b 1 test-nbsvm.txt model.logreg " + out)
        
if __name__ == "__main__":

    main("D:\\tmp\\train-pos.txt","D:\\tmp\\train-neg.txt", "D:\\tmp\\test-pos.txt", 
         "D:\\tmp\\test-neg.txt", "D:\\tmp\\out.txt","C:\\liblinear\\" , "123" )