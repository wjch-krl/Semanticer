// This is the main DLL file.

// Semanticer.Doc2VecWrapper.h
#include "Doc2Vec.h"
#include "TaggedBrownCorpus.h"
#include "TrainModelThread.h"

#pragma once

using namespace System;

namespace SemanticerDoc2VecWrapper
{
	public ref class SimmilarItem
	{
	public:
		SimmilarItem()
		{
			
		}
		SimmilarItem(knn_item_t item)
		{
			Index = item.idx;
			Message = gcnew String(item.word);
			Simmilarity = item.similarity;
		}
		property long long Index;
		property String^ Message;
		property real Simmilarity;
	};


	public ref class Doc2VecWrapper
	{
		Doc2Vec* doc2Vec;
	public:
		Doc2VecWrapper(int dim)
		{
			doc2Vec = new Doc2Vec();
			this->dimmiensions = dim;

		}

		void Train(String^ train_file,
			int cbow, int hs, int negtive,
			int iter, int window,
			real alpha, real sample,
			int min_count, int threads)
		{
			char* path = ConvertString(train_file);
			doc2Vec->train(path,dimmiensions,cbow,hs,negtive,iter,window,alpha,sample,min_count,threads);
		}

		void Save(String^ path)
		{
			char* cpath = ConvertString(path);
			FILE * fout = fopen(cpath, "wb");
			doc2Vec->save(fout);
			fclose(fout);
		}
		
		void Load(String^ path)
		{
			char* cpath = ConvertString(path);
			FILE * fout = fopen(cpath, "rb");
			doc2Vec->load(fout);
			fclose(fout);
		}

		array<SimmilarItem^> ^ SimmilarDocs(String^ label, int itemCount)
		{
			char* clabel = ConvertString(label);
			knn_item_t* knn_items = new knn_item_t[itemCount];
			doc2Vec->doc_knn_docs(clabel,knn_items,itemCount);
			return ToClrArray(knn_items,itemCount);
		}
		
		array<SimmilarItem^> ^ SimmilarDocs(array<String^> ^document, int itemCount)
		{
			TaggedDocument* doc = CreateDocument(document);
			real* inferVec = new real[itemCount];
			knn_item_t* knn_items = new knn_item_t[itemCount];
			doc2Vec->sent_knn_docs(doc,knn_items,itemCount, inferVec);
			return ToClrArray(knn_items,itemCount);
		}

		array<real> ^ InferDoc(array<String^> ^document, int skip)
		{
			TaggedDocument* doc = CreateDocument(document);
			real* vector = new real[dimmiensions];
			doc2Vec->infer_doc(doc, vector, skip);
			return ToClrArray(vector, dimmiensions);
		}

	private:
		int dimmiensions;

		TaggedDocument* CreateDocument(array<String^> ^document)
		{
			TaggedDocument* doc = new TaggedDocument();
			doc->m_word_num = document->Length;
			BuildDoc(doc, document);
			return doc;
		}

		char* ConvertString(String^ string)
		{
			char* cString = new char[string->Length + 1];
			sprintf(cString, "%s", string);
			return cString;
		}

		array<SimmilarItem^> ^ ToClrArray(knn_item_t* knn_items, int size)
		{
			array<SimmilarItem ^> ^ items = gcnew array<SimmilarItem ^>(size);
			for (int i = 0; i < size; i++)
			{
				items[i] = gcnew SimmilarItem(knn_items[i]);
			}
			delete[] knn_items;
			return items;
		}
		
		array<real> ^ ToClrArray(real* src_array, int size)
		{
			array<real> ^ items = gcnew array<real>(size);
			for (int i = 0; i < size; i++)
			{
				items[i] = src_array[i];
			}
			delete[] src_array;
			return items;
		}

		void BuildDoc(TaggedDocument* doc, array<String^> ^document) {

			for (int i = 0; i < doc->m_word_num; i++)
			{
				doc->m_words[i] = ConvertString(document[i]);
			}
		}
	};
}
