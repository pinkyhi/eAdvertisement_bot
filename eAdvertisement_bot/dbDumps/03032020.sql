-- MySQL dump 10.13  Distrib 8.0.16, for Win64 (x86_64)
--
-- Host: localhost    Database: eadvertisement_bot
-- ------------------------------------------------------
-- Server version	8.0.16

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `admessage`
--

DROP TABLE IF EXISTS `admessage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `admessage` (
  `admessage_id` int(11) NOT NULL,
  `advertisement_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`admessage_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admessage`
--

LOCK TABLES `admessage` WRITE;
/*!40000 ALTER TABLE `admessage` DISABLE KEYS */;
INSERT INTO `admessage` VALUES (12,10),(13,10);
/*!40000 ALTER TABLE `admessage` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `advertisement`
--

DROP TABLE IF EXISTS `advertisement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `advertisement` (
  `channel_id` bigint(20) NOT NULL,
  `date_time` datetime NOT NULL,
  `top` int(11) DEFAULT '1',
  `alive` int(11) DEFAULT '24',
  `publication_snapshot` longtext,
  `user_id` int(11) NOT NULL,
  `advertisement_status_id` int(11) NOT NULL DEFAULT '1',
  `price` int(11) NOT NULL,
  `advertisement_id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`advertisement_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `advertisement`
--

LOCK TABLES `advertisement` WRITE;
/*!40000 ALTER TABLE `advertisement` DISABLE KEYS */;
INSERT INTO `advertisement` VALUES (-1001210790589,'2020-02-03 10:00:00',1,24,'{\"Publication_Id\":60,\"Name\":\"newPost\",\"User_Id\":933004747,\"Text\":\"It\\u0027s my post!!!!\",\"Media\":[{\"Media_Id\":43,\"Publication_Id\":60,\"Path\":\"AgACAgIAAxkBAAIVGV5b0L14pBzz73GA06PO6oQm1O_YAAKVrTEb8w3gSrNU8g5zE_s70TR6kS4AAwEAAwIAA3kAA2lyAAIYBA\"}],\"Buttons\":null}',933004747,4,667,9),(-1001210790589,'2020-02-03 10:00:00',1,24,'{\"Publication_Id\":62,\"Name\":\"Album\",\"User_Id\":933004747,\"Text\":\"Text\",\"Media\":[{\"Media_Id\":46,\"Publication_Id\":62,\"Path\":\"AgACAgIAAxkBAAIVo15dmsbVlVF4gGyLb9hg_PKbkdS3AAJorDEb8w3wSlr-3xdgvIJ-rmnBDwAEAQADAgADeQAD-xkFAAEYBA\"},{\"Media_Id\":47,\"Publication_Id\":62,\"Path\":\"AgACAgIAAxkBAAIVp15dmtRB6IwRdCZVV0t_65b2vd7KAAJprDEb8w3wShslEWBODi51jKx4kS4AAwEAAwIAA3kAA1eCAAIYBA\"}],\"Buttons\":null}',933004747,4,667,10);
/*!40000 ALTER TABLE `advertisement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `advertisement_status`
--

DROP TABLE IF EXISTS `advertisement_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `advertisement_status` (
  `advertisement_status_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`advertisement_status_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `advertisement_status`
--

LOCK TABLES `advertisement_status` WRITE;
/*!40000 ALTER TABLE `advertisement_status` DISABLE KEYS */;
INSERT INTO `advertisement_status` VALUES (1,'Awaiting'),(2,'Accepted'),(3,'Declined'),(4,'Working'),(5,'Successful'),(6,'Interrupted'),(7,'New'),(8,'Init Confirmation'),(9,'Own'),(10,'Post trouble'),(11,'Messages[] is empty');
/*!40000 ALTER TABLE `advertisement_status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `autobuy`
--

DROP TABLE IF EXISTS `autobuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `autobuy` (
  `autobuy_id` int(11) NOT NULL AUTO_INCREMENT,
  `publication_snapshot` longtext,
  `interval` int(11) NOT NULL,
  `min_price` int(11) NOT NULL DEFAULT '0',
  `max_price` int(11) NOT NULL,
  `max_cpm` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `state` int(11) NOT NULL DEFAULT '0',
  `balance` int(11) NOT NULL DEFAULT '0',
  `name` varchar(64) NOT NULL DEFAULT 'New autobuy',
  `daily_interval_from` time NOT NULL,
  `daily_interval_to` time NOT NULL,
  PRIMARY KEY (`autobuy_id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `autobuy`
--

LOCK TABLES `autobuy` WRITE;
/*!40000 ALTER TABLE `autobuy` DISABLE KEYS */;
INSERT INTO `autobuy` VALUES (1,'{\"Publication_Id\":45,\"Name\":\"newPost\",\"User_Id\":458816638,\"Text\":null,\"Media\":null,\"Buttons\":null}',1,1,20000,6000,458816638,0,0,'My first autobuy','00:00:00','00:00:00'),(8,NULL,0,0,0,0,458816638,0,0,'New autobuy','06:00:00','22:00:00'),(9,NULL,12,400,10000,120,357927075,1,16000,'Автозакуп','10:00:00','24:00:00'),(10,NULL,0,0,0,0,811048411,0,0,'New autobuy','00:00:00','00:00:00'),(11,NULL,0,0,0,0,357927075,0,0,'New autobuy','00:00:00','00:00:00'),(12,'{\"Publication_Id\":56,\"Name\":\"newPost\",\"User_Id\":811048411,\"Text\":\"peqiddnebzjs\",\"Media\":null,\"Buttons\":null}',0,0,0,140,811048411,1,0,'1245','00:00:00','00:00:00'),(13,NULL,0,0,0,0,357927075,0,0,'New autobuy','00:00:00','00:00:00'),(14,NULL,0,1,666,600,437395227,1,666,'New autobuy','17:00:00','18:00:00'),(15,NULL,0,0,0,0,437395227,0,0,'New autobuy','00:00:00','00:00:00');
/*!40000 ALTER TABLE `autobuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `autobuy_channel`
--

DROP TABLE IF EXISTS `autobuy_channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `autobuy_channel` (
  `autobuy_id` int(11) NOT NULL,
  `channel_id` bigint(20) NOT NULL,
  PRIMARY KEY (`autobuy_id`,`channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `autobuy_channel`
--

LOCK TABLES `autobuy_channel` WRITE;
/*!40000 ALTER TABLE `autobuy_channel` DISABLE KEYS */;
INSERT INTO `autobuy_channel` VALUES (1,-1001456914809),(1,-1001335316790),(12,-1001335316790),(14,-1001335316790);
/*!40000 ALTER TABLE `autobuy_channel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `button`
--

DROP TABLE IF EXISTS `button`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `button` (
  `button_id` int(11) NOT NULL AUTO_INCREMENT,
  `publication_id` int(11) DEFAULT NULL,
  `text` varchar(32) NOT NULL,
  `url` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`button_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `button`
--

LOCK TABLES `button` WRITE;
/*!40000 ALTER TABLE `button` DISABLE KEYS */;
INSERT INTO `button` VALUES (5,45,'Button 1','http://example1.com'),(6,45,'Button 2','http://example2.com'),(8,36,'На сайт','http://itravel.im');
/*!40000 ALTER TABLE `button` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `category` (
  `category_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (4,'Humor'),(5,'Comics'),(6,'Drugs'),(7,'Politics');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channel`
--

DROP TABLE IF EXISTS `channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `channel` (
  `channel_id` bigint(20) NOT NULL,
  `link` varchar(256) DEFAULT NULL,
  `subscribers` int(11) NOT NULL DEFAULT '0',
  `coverage` int(11) NOT NULL DEFAULT '0',
  `cpm` int(11) DEFAULT NULL,
  `name` varchar(64) NOT NULL,
  `description` varchar(1024) DEFAULT NULL,
  `ban` tinyint(1) NOT NULL DEFAULT '0',
  `user_id` bigint(20) NOT NULL,
  `date` datetime DEFAULT CURRENT_TIMESTAMP,
  `price` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`channel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel`
--

LOCK TABLES `channel` WRITE;
/*!40000 ALTER TABLE `channel` DISABLE KEYS */;
INSERT INTO `channel` VALUES (-1001490275304,'https://t.me/joinchat/AAAAAFjTy-g9D20fp_fY4w',2,0,NULL,'Vaniiidlo',NULL,0,517407871,'2019-11-13 05:57:21',0),(-1001488041253,'https://t.me/joinchat/AAAAAFixtSWUaYq_RFusGw',4,2,200,'TestPrivacy','suck that',0,933004747,'2019-11-12 18:34:01',190),(-1001471666826,'https://t.me/joinchat/AAAAAFe32or89lJ6nlOPng',2,0,200,'I love my kat☺️?','‘Super myau’',0,401093470,'2019-11-20 23:03:26',180),(-1001456914809,'https://t.me/joinchat/AAAAAFbWwXk7swggjyad9Q',12009,6928,140,'веские поводы бросить универ','класс',0,357927075,'2020-02-19 18:00:32',969),(-1001436122434,'https://t.me/joinchat/AAAAAFWZfUL4YxNNvPi65A',2,0,100,'Tttttf',NULL,0,933004747,'2019-11-18 10:10:06',170),(-1001354683486,'https://t.me/joinchat/AAAAAFC-1F4O3GARNcH2QA',20120,6727,NULL,'Важные Статистические Опросы (ВСО)',NULL,0,811048411,'2020-02-19 17:57:49',0),(-1001335316790,'https://t.me/joinchat/AAAAAE-XUTZtVdxgEfBuAA',4,0,100,'тысяча хуев тебе в рот виталик',NULL,0,357927075,'2019-11-18 13:48:36',160),(-1001322943856,'https://t.me/joinchat/AAAAAE7ahXBeGUfTgAIwXw',3670,1005,120,'comicschan',NULL,0,811048411,'2020-02-19 18:01:14',120),(-1001299142810,'https://t.me/joinchat/AAAAAE1vWJrsKYxAK5CG3g',6610,1712,120,'Твоя Доза Деградации',NULL,0,811048411,'2020-02-19 19:50:16',205),(-1001281342955,'https://t.me/joinchat/AAAAAExfvevDvegLORYxcg',16088,2461,100,'тысяча хуёв тебе в рот, олег',NULL,0,811048411,'2019-11-13 14:14:08',120),(-1001210790589,'https://t.me/joinchat/AAAAAEgrMr184y3Rx7n7aA',3,0,220,'Anal','Anal',0,458816638,'2020-02-24 20:44:59',667),(-1001204934258,'https://t.me/joinchat/AAAAAEfR1nJatQ_W-3GNtw',3,3,NULL,'LastPrivacyChannelTes',NULL,0,933004747,'2019-12-12 16:15:13',140),(-1001152144911,'https://t.me/joinchat/AAAAAESsVg9786UpTOqYMg',2,0,NULL,'Hhsh',NULL,0,933004747,'2019-11-20 22:57:56',150);
/*!40000 ALTER TABLE `channel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channel_category`
--

DROP TABLE IF EXISTS `channel_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `channel_category` (
  `channel_id` bigint(20) NOT NULL,
  `category_id` int(11) NOT NULL,
  PRIMARY KEY (`channel_id`,`category_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channel_category`
--

LOCK TABLES `channel_category` WRITE;
/*!40000 ALTER TABLE `channel_category` DISABLE KEYS */;
INSERT INTO `channel_category` VALUES (-1001471666826,6),(-1001281342955,4),(-1001281342955,5);
/*!40000 ALTER TABLE `channel_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `input`
--

DROP TABLE IF EXISTS `input`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `input` (
  `input_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` bigint(20) NOT NULL,
  `sum` double NOT NULL,
  `inputed` int(11) NOT NULL,
  `description` varchar(64) NOT NULL,
  PRIMARY KEY (`input_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `input`
--

LOCK TABLES `input` WRITE;
/*!40000 ALTER TABLE `input` DISABLE KEYS */;
INSERT INTO `input` VALUES (5,458816638,1,1,'1');
/*!40000 ALTER TABLE `input` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `media`
--

DROP TABLE IF EXISTS `media`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `media` (
  `media_id` int(11) NOT NULL AUTO_INCREMENT,
  `publication_id` int(11) DEFAULT NULL,
  `path` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`media_id`)
) ENGINE=InnoDB AUTO_INCREMENT=48 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `media`
--

LOCK TABLES `media` WRITE;
/*!40000 ALTER TABLE `media` DISABLE KEYS */;
INSERT INTO `media` VALUES (16,45,'AgACAgIAAxkBAAIKoF45jbN-s0lqdfsHw1WxtJ0bThndAALPrDEbaFPRScGIg02pjoG27FnLDgAEAQADAgADeQAD78YBAAEYBA'),(23,49,'AgACAgIAAxkBAAINql5BSYEHknUW6YDVMmMmKkHdjR2PAAI3rDEb-P4ISm6SvUlDeRozY2PLDgAEAQADAgADeAADcxECAAEYBA'),(24,49,'AgACAgIAAxkBAAINs15BSY2f5sAaiscvya4g5BoaCHbOAAI2rDEb-P4ISsUB_AuJnf6wEpTCDwAEAQADAgADeQADzEkEAAEYBA'),(25,49,'AgACAgIAAxkBAAINuF5BSZbjnrC42pGFoO_fxJ3OMIvkAAI7rDEb-P4ISiNokFial7qYGqfCDwAEAQADAgADeAADgksEAAEYBA'),(26,50,'AgACAgIAAxkBAAIO1F5NSKxgCeGrpQzJiQkSiarPwxl8AAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(27,50,'AgACAgIAAxkBAAIO015NSBd9a6YrqqIiNX9U7HKPmPl3AAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(28,51,'AgACAgIAAxkBAAIO6V5NSTLc51-OVFT7h65Se6E3n_9hAAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(29,51,'AgACAgIAAxkBAAIO7l5NSUbWCoYh0w7C0Zm9DkmQMKIbAAIsrTEbacJoSh5mnmLjpkPJHgpxkS4AAwEAAwIAA3kAA5oTAAIYBA'),(30,52,'AgACAgIAAxkBAAIPDF5NSuY8As0NzlGzGR4Cpw64dX4DAAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(31,52,'AgACAgIAAxkBAAIPEF5NSw0jHOZgVNSCdLVdhZLPn6ucAAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(32,54,'AgACAgIAAxkBAAIPMl5NTbWWkfqI1HoEBpADSW8VcsGiAAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(33,54,'AgACAgIAAxkBAAIPNl5NTcEfspzqu9ArqwpSz5vE7GqaAAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(34,54,'AgACAgIAAxkBAAIPOF5NTdfjtfQw2x28Yz2jpX1GZvLFAAIDrTEbx-ZpSqAfn0wnv_m6uRXBDgAEAQADAgADeAADsxoDAAEYBA'),(37,56,'AgACAgIAAxkBAAIQrF5Nknt1chD-QCZoJzA3DAmAeTF5AAJlrDEbmcNpStmLebVC1UlocEbLDgAEAQADAgADeAAD-HgCAAEYBA'),(38,56,'AgACAgIAAxkBAAIQsF5NkofkmJ7ICCn8Key0ERhommYYAALGrTEbdtFwSswCckwJbhdMsHVcDwAEAQADAgADeQADU8IFAAEYBA'),(41,37,'AgACAgIAAxkBAAITZ15VBMD9NhWCPBfM8jjPaaHqSNqYAAKCrDEbmqupSkc7ezupPUYNwTF6kS4AAwEAAwIAA3gAA11GAAIYBA'),(42,36,'AgACAgIAAxkBAAIUg15bt93LjhodqY9wOaL7vgy1MqLSAALiqzEbdGngSjHt-QkX50-NzGNcDwAEAQADAgADeQADBCUGAAEYBA'),(43,60,'AgACAgIAAxkBAAIVGV5b0L14pBzz73GA06PO6oQm1O_YAAKVrTEb8w3gSrNU8g5zE_s70TR6kS4AAwEAAwIAA3kAA2lyAAIYBA'),(44,61,'AgACAgIAAxkBAAIVg15dmkf-paNPdjaMN__hzS96ccnPAAICrzEbPb3oSmcXoMK3Bp7fmGdcDwAEAQADAgADeQADRC0GAAEYBA'),(45,61,'AgACAgIAAxkBAAIVh15dmlqgI1K0euLlKUgtAVzJfrNKAAL7rDEb4DHRSlEwUhyTq8rXLhrBDgAEAQADAgADeQADVmkDAAEYBA'),(46,62,'AgACAgIAAxkBAAIVo15dmsbVlVF4gGyLb9hg_PKbkdS3AAJorDEb8w3wSlr-3xdgvIJ-rmnBDwAEAQADAgADeQAD-xkFAAEYBA'),(47,62,'AgACAgIAAxkBAAIVp15dmtRB6IwRdCZVV0t_65b2vd7KAAJprDEb8w3wShslEWBODi51jKx4kS4AAwEAAwIAA3kAA1eCAAIYBA');
/*!40000 ALTER TABLE `media` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `place`
--

DROP TABLE IF EXISTS `place`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `place` (
  `time` time NOT NULL,
  `channel_id` bigint(20) NOT NULL,
  `place_id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`time`,`channel_id`),
  UNIQUE KEY `place_id_UNIQUE` (`place_id`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `place`
--

LOCK TABLES `place` WRITE;
/*!40000 ALTER TABLE `place` DISABLE KEYS */;
INSERT INTO `place` VALUES ('13:45:00',-1001456914809,26),('19:27:00',-1001456914809,27),('18:46:00',-1001354683486,29),('10:00:00',-1001210790589,34);
/*!40000 ALTER TABLE `place` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `publication`
--

DROP TABLE IF EXISTS `publication`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `publication` (
  `publication_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) DEFAULT NULL,
  `user_id` bigint(20) DEFAULT NULL,
  `text` varchar(2048) DEFAULT NULL,
  PRIMARY KEY (`publication_id`)
) ENGINE=InnoDB AUTO_INCREMENT=63 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `publication`
--

LOCK TABLES `publication` WRITE;
/*!40000 ALTER TABLE `publication` DISABLE KEYS */;
INSERT INTO `publication` VALUES (36,'Реклама путёвки на море',517407871,'Покупайте горящие путёвки !!!\nЗаходи на itravel.im и получи скидку 20% на любой тур ?'),(37,'newPost',517407871,'Ваня аааа'),(45,'newPost',458816638,NULL),(49,'Пост веские',357927075,'Эй, ты студент?\r \r Устал с'),(50,'newPost',357927075,NULL),(51,'newPost',357927075,'oyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysodoyoysoydoysiydoydysiysitdoysotsoysitsotsotsoysitsitsitsitsiysod'),(52,'newPost',357927075,'123'),(54,'newPost',357927075,'12r5'),(56,'newPost',811048411,'peqiddnebzjs'),(57,'newPost',517407871,NULL),(58,'newPost',517407871,NULL),(59,'newPost',517407871,NULL),(60,'newPost',933004747,'It\'s my post!!!!'),(61,'Album',458816638,NULL),(62,'Album',933004747,'Text');
/*!40000 ALTER TABLE `publication` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user` (
  `user_id` bigint(20) NOT NULL,
  `phone` varchar(24) DEFAULT NULL,
  `nickname` varchar(64) DEFAULT NULL,
  `balance` int(11) NOT NULL DEFAULT '0',
  `ban` bit(1) NOT NULL,
  `firstname` varchar(64) DEFAULT NULL,
  `lastname` varchar(64) DEFAULT NULL,
  `language` varchar(32) DEFAULT NULL,
  `stopped` bit(1) NOT NULL,
  `user_state_id` bigint(20) NOT NULL,
  `object_id` bigint(20) NOT NULL DEFAULT '0',
  `tag` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (357927075,NULL,'Sub2Ch',982087,_binary '\0','SHIIIITT',NULL,NULL,_binary '\0',3,9,NULL),(401093470,NULL,NULL,1000000,_binary '\0','Вероника','Балаклицкая','en',_binary '',0,0,NULL),(437395227,NULL,'kyoshaz',0,_binary '\0','Dvd',NULL,'ru',_binary '\0',0,14,NULL),(458816638,NULL,'pinky_hi',999694,_binary '\0','Иван','Владимиров','ru',_binary '',3,61,NULL),(517407871,NULL,'zlobste',999333,_binary '\0','Николай','Крайнюк','ru',_binary '\0',0,36,NULL),(811048411,NULL,'olejchanskiy',1000000,_binary '\0','олег','(не олег)',NULL,_binary '\0',0,-1001281342955,NULL),(933004747,NULL,'memniyboh',990662,_binary '\0','Мемный Бох',NULL,'ru',_binary '\0',3,62,NULL);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_state`
--

DROP TABLE IF EXISTS `user_state`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `user_state` (
  `user_state_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `state` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`user_state_id`)
) ENGINE=InnoDB AUTO_INCREMENT=902 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_state`
--

LOCK TABLES `user_state` WRITE;
/*!40000 ALTER TABLE `user_state` DISABLE KEYS */;
INSERT INTO `user_state` VALUES (0,'Null'),(1,'Waiting for a new channel'),(2,'Channel menu'),(3,'Waiting for a channel to buy '),(4,'Autobuy menu'),(5,'Add channels to autobuy menu'),(101,'101*=add img to post with id *'),(102,'102*=add btn to post with id *'),(103,'103*=chng text in post with id *'),(104,'104*=cnhg name of post with id *'),(201,'201*=add place to chnl with id *'),(202,'202*=chng cpm to chnl with id *'),(203,'203*=chng desc to chnl with id *'),(204,'Delete channel'),(301,'Change manual price interval'),(401,'Change autobuy\'s name'),(402,'Change autobuy\'s max cpm'),(403,'Change Autobuy Min Max Price'),(404,'Change autobuy\'s interval'),(405,'Adding balance to autobuy'),(406,'Change daily time interval in ab'),(901,'Waiting for a time for own sold');
/*!40000 ALTER TABLE `user_state` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-03-03 21:22:15
