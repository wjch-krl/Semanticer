library(e1071)
library(RJDBC)
library(psych)
library(fBasics)
library(stringr)
 # lang = "pl-PL"
 # trade = 5
 # findBestStats = FALSE
getAllMessages = FALSE
mGamma = 0.3535534
mCost = 0.1767767
mType ="C-classification" 
mKernel = "radial"

drv <- JDBC("org.postgresql.Driver", "postgresql-9.4-1201.jdbc41.jar")
conn <- dbConnect(drv, "jdbc:postgresql:SocialBITest", "postgres", "postgres")
train_data_query <- paste("SELECT pos_normalizemessage,pos_message, pos_marktype, pos_mark, train_isused, pk_train_data_id from post, train_data 
where pk_pos_id = pospk_pos_id and (train_isused or train_isused is null) and 
lng_pk_lng_id = (select \"LNG_ID\" from \"Languages\" where \"LNG_Name\" = '",lang,"') and trd_id = ",trade,";", sep="")
all_train_data_query <- paste("SELECT pos_normalizemessage, pos_message, pos_marktype, pos_mark, train_isused, pk_train_data_id from post, train_data where pk_pos_id = pospk_pos_id  and lng_pk_lng_id = (select \"LNG_ID\" from \"Languages\" where \"LNG_Name\" = '",lang,"') and trd_id = ",trade,";", sep="")
d <- NULL
if (getAllMessages)
{
	d <- dbGetQuery(conn, all_train_data_query) 
} else
{
	d <- dbGetQuery(conn, train_data_query)
}

gm_mean = function(x, na.rm=TRUE)
{ 
  exp(sum(log(x[x > 0]), na.rm=na.rm) / length(x))
}
 
create.stats = function(dataFrame)
{
	maxStat <<- 0
	minStat <<- 0
	msg_count = nrow(dataFrame)
	stat_data = NULL
	for ( i in 1:msg_count) 
	{
		msg = dataFrame[i,"pos_normalizemessage"]
		markType = dataFrame[i,"pos_marktype"]
		id = dataFrame[i,"pk_train_data_id"];
		isUsed = ifelse(dataFrame[i,"train_isused"] == "t", 1, 0);
		db_notes <- dbGetQuery(conn, paste("SELECT note from f_note('",msg,"','",lang,"') where note is not null ;", sep=""))
		
		if (nrow(db_notes) >= 1 )
		{	
			db_notes = as.numeric(db_notes[,1])
			quant = quantile(db_notes)
			
			qustionMarks = str_count(dataFrame[i,"pos_message"],"\\?")
			exclamationMarks = str_count( dataFrame[i,"pos_message"],"!")
			
			emt_notes <- dbGetQuery(conn, paste("SELECT ocena from f_rateemoticons('",msg,"') ;", sep=""))
			emt_notes_sum = ifelse(nrow(emt_notes) > 0,sum(emt_notes),0)
			
			desc = c(markType,length(db_notes),mean(db_notes),sd(db_notes),
				median(db_notes),as.numeric(quant[2]),as.numeric(quant[4]),
				as.numeric(quant[4]) - as.numeric(quant[2]),kurtosis(db_notes, method = "fisher"),
				skewness(db_notes,method = "fisher"),max(db_notes),min(db_notes),
				range(db_notes)[2]- range(db_notes)[1],sum(db_notes),var(db_notes),emt_notes_sum, qustionMarks, exclamationMarks, id, isUsed)
		
			if (is.null(stat_data)) 
			{
				stat_data = desc
			} else 
			{
				stat_data = cbind(stat_data, desc)
			}
		}
	}
	retval = t(stat_data)
	retval[!is.finite(retval)] <- 0
	rownames(retval) <- NULL
	retval = as.data.frame(retval)
	colnames(retval) <- c("result", "nrow", "mean", "sd", "median", "quant[2]", "quant[4]", "IQR", "kurtosis", "skewness", "max", "min", "range", "sum", "var","emoticon","qustionMarks","exclamationMarks" ,"pk_train_data_id", "isUsed")
	for ( i in 2:(ncol(retval)-2)) 
	{
		maxStat <<- cbind(maxStat,max(retval[,i]))
		minStat <<- cbind(minStat,min(retval[,i]))
		retval[,i] = (retval[,i] - min(retval[,i]))/(max(retval[,i]) - min(retval[,i]))
    }

	return (retval)
}

read.libsvm = function( filename, dimensionality ) {

	content = readLines( filename )
	num_lines = length( content )
	yx = matrix( 0, num_lines, dimensionality + 1 )

	# loop over lines
	for ( i in 1:num_lines ) {
	
		# split by spaces
		line = as.vector( strsplit( content[i], ' ' )[[1]])
	
		# save label
		yx[i,1] = as.numeric( line[[1]] )
	
		# loop over values
		for ( j in 2:length( line )) {
	
			# split by colon
			index_value = strsplit( line[j], ':' )[[1]]

			index = as.numeric( index_value[1] ) + 1		# +1 because label goes first
			value = as.numeric( index_value[2] )

			yx[i, index] = value
		}
	}
	
	return( yx )
}

write.libsvm = function( filename, y, x ) {

	# open an output file
	f = file( filename, 'w' )
	# loop over examples
	for ( i in 1:nrow( x )) {

		# find indexes of nonzero values
		indexes = which( as.logical( x[i,] ))

		# those nonzero values
		values = x[i, indexes]

		# concatenate to the target format ( "1:6 3:77 6:8" )		
		iv_pairs = paste( indexes, values, sep = ":", collapse = " " )
		
		# add label in the front and newline at the end
		output_line = paste( y[i], " ", iv_pairs, "\n", sep = "" )
		
		# write to file
		cat( output_line, file = f )
		
		# print progress
		if ( i %% 1000 == 0 ) {
			print( i )
		}
	}
	
	# close the connection
	close( f )
}

#funkcja do przekształcenia wyniku regresji do klas im odpowiadającym
as.markType = function (dat){

	vec = numeric(length(dat))
	for(i in 1:length(dat))
	{
		d = dat[i]
		markType = 2
		if(d<1.6)
		{
			markType = 1 
		}
		else if (d > 2.4)
		{
			markType = 3
		} 
		vec[i] = markType
	}
	return (vec)
}

find.bestStats = function (stats)
{
	allowedStats = seq(2,ncol(stats)-2,1)
	bestPrecision = 0;
	bestStats = allowedStats	
	
	for (j in 2: length(allowedStats))
	{
		combinations = combn (allowedStats,j)
		for (i in 1:ncol(combinations))
		{
			train_data = stats[,cbind(1,t(combinations[,i]))]
			## 75% of the sample size
			smp_size <- floor(0.75 * nrow(train_data))
			## set the seed to make your partition reproducible
			set.seed(123)
			train_ind <- sample(seq_len(nrow(train_data)), size = smp_size)

			train <- train_data[train_ind,]
			test <- train_data[-train_ind,]

			model <- svm( train[,-1],train[,1], , gamma = mGamma, cost = mCost, type=mType, kernel = mKernel)
			prediction <- predict(model,test[,-1])
			tab <- table(prediction, test[,1])
			precision <- sum(tab[row(tab) == (col(tab))])/sum(tab)
			if (precision> bestPrecision)
			{
				bestPrecision = precision
				bestStats = combinations[,i]
			}
		}
	}
	return (bestStats)
}

find.bestStatsAgresive = function (stats)
{
	allowedStats = seq(2,ncol(stats)-2,1)
	bestPrecision = 0;
	bestStats = allowedStats	
	
	for (j in 2: length(allowedStats))
	{
		combinations = combn (allowedStats,j)
		for (i in 1:ncol(combinations))
		{
			train_data = stats[,cbind(1,t(combinations[,i]))]
			## 75% of the sample size
			smp_size <- floor(0.75 * nrow(train_data))
			## set the seed to make your partition reproducible
			set.seed(123)
			train_ind <- sample(seq_len(nrow(train_data)), size = smp_size)

			train <- train_data[train_ind,]
			test <- train_data[-train_ind,]

			model <- svm( train[,-1],train[,1], , gamma = mGamma, cost = mCost, type=mType, kernel = mKernel)
			prediction <- predict(model,test[,-1])
			tab <- table(prediction, test[,1])
			precision <- (tab[1,1]+tab[3,3])/(sum(tab[,1])+sum(tab[,3]))
			print(usedStats)
			print(tab)
			print(precision)
			
			if (precision> bestPrecision)
			{
				bestPrecision = precision
				bestStats = combinations[,i]
			}
		}
	}
	return (bestStats)
}

corssvalidate.stats = function (stats)
{
	currentlyUsed = stats[stats$isUsed == 1,]
	notUsed = stats[stats$isUsed != 1,]
	nposts<-notUsed[sample(nrow(notUsed)),]
	
	bestTrainData = currentlyUsed

	#Create 10 equally size folds
	breaksCount = 10
	if(nrow(notUsed) > 1)
	{
		if (nrow(notUsed) < breaksCount)
		{
			breaksCount = nrow(notUsed)
		}
		
		set.seed(23)
		smp_size <- floor(0.75 * nrow(currentlyUsed))
		train_ind <- sample(seq_len(nrow(currentlyUsed)), size = smp_size)
		trainData = currentlyUsed[train_ind,]
		testData =  currentlyUsed[-train_ind,]

		model <- svm( trainData[,-1],trainData[,1],gamma = mGamma, cost = mCost, type=mType, rnel = mKernel)
		prediction <- predict(model, testData[,-1])
		tab <- table(prediction, testData[,1])
		bestPrecision <- sum(tab[row(tab) == (col(tab))])/sum(tab)
		
		folds <- cut(seq(1,nrow(nposts)),breaks=breaksCount,labels=FALSE)
		#10 fold cross validation
		postsToIgnore = NULL
		postsToUse = NULL
		for(i in 1:breaksCount)
		{
			#Segment your data by fold using the which() function 
			testIndexes <- which(folds==i,arr.ind=TRUE)
			trainData = currentlyUsed[train_ind,]
			trainData = cbind(nposts[-testIndexes,],trainData)
			
			model <- svm( trainData[,-1],trainData[,1],gamma = mGamma, cost = mCost, type=mType, rnel = mKernel)
			prediction <- predict(model, testData[,-1])
			tab <- table(prediction, testData[,1])
			precision <- sum(tab[row(tab) == (col(tab))])/sum(tab)

			if ( precision < bestPrecision)
			{
				postsToIgnore=c(postsToIgnore,trainData[,"pk_train_data_id"])
			} else
			{
				postsToUse=c(postsToUse,trainData[,"pk_train_data_id"])
				bestTrainData = cbind(nposts[-testIndexes,],bestTrainData)
			}
		}
		if (length(postsToIgnore) > 0){
			postIgnoreQuery = paste("update train_data set train_isused = false where pk_train_data_id in (",paste (postsToIgnore,  collapse = ", "),");")
			dbSendUpdate(conn,postIgnoreQuery)
		}
		if (length(postsToUse) > 0){
			postUseQuery = paste("update train_data set train_isused = true where pk_train_data_id in (",paste (postsToUse,  collapse = ", "),");")
			dbSendUpdate(conn,postUseQuery)
		}
	}
	return (bestTrainData)
}

corssvalidate.allstats = function (stats)
{
	nposts<-stats[sample(nrow(stats)),]
	bestPrecision = 0;
	bestTrainData = stats	

	#Create 10 equally size folds
	folds <- cut(seq(1,nrow(nposts)),breaks=10,labels=FALSE)
	#10 fold cross validation
	for(i in 1:10)
	{
		#Segment your data by fold using the which() function 
		testIndexes <- which(folds==i,arr.ind=TRUE)
		testData <- nposts[testIndexes, ]
		trainData <- nposts[-testIndexes, ]
		
		model <- svm( trainData[,-1],trainData[,1],gamma = mGamma, cost = mCost, type=mType, rnel = mKernel)
		prediction <- predict(model, testData[,-1])
		tab <- table(prediction, testData[,1])
		precision <- sum(tab[row(tab) == (col(tab))])/sum(tab)

		if ( precision> bestPrecision)
		{
			bestPrecision = precision
			bestTrainData = trainData
		}
	}
	return (bestTrainData)
}

transform.textAsDouble = function (posts)
{
	msg_count = nrow(posts)
	stat_data = NULL
	l = list()
	for ( i in 1:msg_count) 
	{
		msg = posts[i,"pos_normalizemessage"]
		splitted = unlist(strsplit(msg," ",fixed=TRUE))
		msg_len = length(splitted)
		for(j in 1:msg_len)
		{
			if (is.null(l[splitted[j]]))
			{
				l[[splitted[j]]] = 1
			} else {
				l[[splitted[j]]] = l[[splitted[j]]] + 1
			}
		}
	}
	transformed = list()
	for ( i in 1:msg_count) 
	{
		msg = posts[i,"pos_normalizemessage"]
		markType = posts[i,"pos_marktype"]
		# id = posts[i,"pk_train_data_id"];
		# isUsed = ifelse(posts[i,"train_isused"] == "t", 1, 0);
		splitted = unlist(strsplit(msg," ",fixed=TRUE))
		msg_len = length(splitted)
		msg_vect = list()
		for(j in 1:msg_len)
		{
		    indexes = which(names(l)==splitted[j])
			if (length (indexes) > 0){ 
				msg_vect[[which(names(l)==splitted[j])]] = sum(splitted == splitted[j]) 
			}
		}
		transformed[[i]] = msg_vect
	}
	return (transformed)
}

classificationMatrixToString = function(tab){
	return (paste("{{",paste(tab[1,],collapse=","),"},{",paste(tab[2,],collapse=","),"},{",paste(tab[3,],collapse=","),"}}"))
}

testDataOrg = create.stats(d)
testData <- corssvalidate.stats(testDataOrg)

usedStats = c(1,2,5,8,9,10,11,15)
if(findBestStats)
{
	usedStats = find.bestStatsAgresive(testData)
	usedStats = c(1,usedStats)
} 

tmpData = testData[,usedStats[-1]]
x= as.numeric(as.matrix(tmpData))
b <- matrix(x, ncol = length(usedStats) - 1 )
write.libsvm( paste("SvmTrainData_",lang,"_",trade,".ds", sep=""),testData[,1],b)
trainData = testData[,usedStats]

model = svm(result ~., trainData, gamma = mGamma, cost = mCost, type=mType, 
	kernel = mKernel, probability = TRUE)
prediction = predict(model,testData[,usedStats[-1]], probability = TRUE)
value = testData[,1]
tab = table(prediction, value)
precision <- sum(tab[row(tab) == (col(tab))])/sum(tab)
veryBadPrecent = (tab[1,nrow(tab)] + tab[nrow(tab),1]) / sum(tab)
positive = tab[1,1] / sum(tab[,1])
negative = tab[3,3] / sum(tab[,3]) 
neutral = tab[2,2] / sum(tab[,2])
modelName = paste("Svm_",lang,"_",trade,"_",gsub("[:, ]","_",Sys.time()), sep="")
modelPath = paste(modelName,".model", sep="")
modelDetailInsert = paste("INSERT INTO trained_models(trn_mod_date, ",
            "trn_mod_matrix ,trn_mod_path, trn_mod_lng_id, trn_mod_trd_id)",
			"VALUES ('",Sys.time(),"','", classificationMatrixToString(tab) ,"','",getwd(),"/",
            modelPath,"',(select \"LNG_ID\" from \"Languages\" where \"LNG_Name\" ='", lang,"'),", trade,")", sep="")		
dbSendUpdate(conn,modelDetailInsert)
write.svm(model, svm.file = modelPath, scale.file = paste("Svm_",lang,"_",trade,".scale", sep="")) #TODO delete
modelPath = paste("Svm_",lang,"_",trade,".model", sep="")
write.svm(model, svm.file = modelPath, scale.file = paste(modelName,".scale", sep=""))
tab

set.seed(123)
smp_size <- floor(0.75 * nrow(testData))
train_ind <- sample(seq_len(nrow(testData)), size = smp_size)
trainData = testData[train_ind,]
tstData =  testData[-train_ind,]
model = svm(trainData[,usedStats[-1]], trainData[,1],gamma = 0.65, cost = 2, type="C-classification", kernel = "radial")
prediction = predict(model,tstData[,usedStats[-1]])
value = tstData[,1]
tab = table(prediction, value)
tab

# tstData[abs(as.integer(value) - as.integer(prediction)) == 2,"pk_train_data_id"]
# hist(x,xlab = "Ocena" ,xaxt = 'n' , main = "Ilość postów o danej ocenie (Dane treningowe)")
# axis(side = 1, at =c(1,2,3), labels = c("Possitive","Neutral","Negative"))
# plot(model,data, skewness ~ sd,svSymbol = ".")
# precision <- sum(tab[row(tab) == (col(tab))])/sum(tab)
# x = rowSums (tab)
# x[3] = tab[3,3]/x[3]
# x[2] = tab[2,2]/x[2]
# x[1] = tab[1,1]/x[1]
# veryBad = (tab[3,1]+tab[1,3])/sum(tab)
# x = tmpData[,1]
# y = tmpData [,-1]
# tune = tune.svm(result ~.,data = trainData,gamma = 2^c(-1,0,1), cost = 2^c(-3,-1,-2,0,1))
# hist(as.integer(as.matrix(x)), xlab = "Ocena" ,xaxt = 'n' , main = "Częstotliwość postów o danej ocenie")
usedStats = usedStats[-1]