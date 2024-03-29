﻿using System;
using System.Collections.Generic;
using FrozenCollections;

namespace FrozenCollections.Bench;

internal static class SampleData
{
    private static readonly string[] _raw = new[]
    {
        "0000000000047ac9",
        "000000004008f9a5",
        "00000000401a2a0b",
        "00000000401c687c",
        "000000004406d438",
        "000000004410c342",
        "000000004414fb24",
        "00000000441604df",
        "00000000441bdf19",
        "000000004421814d",
        "000000004422db6a",
        "00000000480728c5",
        "0000000048170ef2",
        "00000000481c978e",
        "000000004c130312",
        "000000004c1631d9",
        "000000004c18159e",
        "000000004c18365e",
        "000000004c28068c",
        "00000002-0000-0000-c000-000000000000",
        "00000002-0000-0ff1-ce00-000000000000",
        "00000002-0000-0ff1-ce00-100000000002",
        "00000002-0000-0ff1-ce00-100000000003",
        "00000002-0000-0ff1-ce00-100000000004",
        "00000002-0000-0ff1-ce00-100000000006",
        "00000002-0000-0ff1-ce00-100000000007",
        "00000002-0000-0ff1-ce00-100000000008",
        "00000002-0000-0ff1-ce00-100000000009",
        "00000002-0000-0ff1-ce00-100000000011",
        "00000002-0000-0ff1-ce00-100000000013",
        "00000002-0000-0ff1-ce00-100000000014",
        "00000002-0000-0ff1-ce00-100000000015",
        "00000002-0000-0ff1-ce00-100000000016",
        "00000002-0000-0ff1-ce00-100000000017",
        "00000002-0000-0ff1-ce00-100000000018",
        "00000002-0000-0ff1-ce00-100000000019",
        "00000002-0000-0ff1-ce00-100000000020",
        "00000002-0000-0ff1-ce00-100000000022",
        "00000003-0000-0000-c000-000000000000",
        "00000003-0000-0ff1-ce00-000000000000",
        "00000004-0000-0ff1-ce00-000000000000",
        "00000005-0000-0000-c000-000000000000",
        "00000005-0000-0ff1-ce00-000000000000",
        "00000006-0000-0ff1-ce00-000000000000",
        "00000007-0000-0000-c000-000000000000",
        "00000007-0000-0ff1-ce00-000000000000",
        "00000008-0000-0000-c000-000000000000",
        "00000009-0000-0000-c000-000000000000",
        "0000000a-0000-0000-c000-000000000000",
        "0000000b-0000-0000-c000-000000000000",
        "0000000c-0000-0000-c000-000000000000",
        "00000011-0000-0000-c000-000000000000",
        "00000014-0000-0000-c000-000000000000",
        "00000015-0000-0000-c000-000000000000",
        "0004c632-673b-4105-9bb6-f3bbd2a927fe",
        "00224774-b968-4d19-a0fd-b0f339eb52a2",
        "005bdcb5-e09a-42ca-93a4-e676c2a2c478",
        "00b263e4-3497-4650-b082-3197cfdfdd7c",
        "012da461-7855-4796-a3fb-8f92929b4b0e",
        "046afeeb-111f-4e07-8564-04816a4823c3",
        "055b7270-5710-4893-bb28-adc85c0f7787",
        "06457fe4-3046-45a6-8d24-471fc0549365",
        "09abbdfd-ed23-44ee-a2d9-a627aa1c90f3",
        "0a0a29f9-0a25-49c7-94bf-c53c3f8fa69d",
        "0bf30f3b-4a52-48df-9a82-234910c4a086",
        "0bf7061f-476d-42ec-9aa2-895496862967",
        "0bf7a9bf-05a9-43d8-b61e-62d3aad62fcd",
        "0d052524-20fa-4a5b-b44b-a70e6705b5eb",
        "0d38933a-0bbd-41ca-9ebd-28c4b5ba7cb7",
        "0d9f9faf-0267-4660-a6cd-72f9b725fe2f",
        "0eaa6b95-4a35-4a5d-9919-e4fc61fb4bdb",
        "0ec044c0-6390-48f8-b76e-331541907259",
        "0fd6e627-4a69-457d-802c-a36a9b14a7ae",
        "104e1f86-f31e-43c3-9f43-05522b871fc8",
        "11b9e49b-c316-456e-a1b3-f62b6abdc2d1",
        "12764ddf-2746-4293-8f63-b79fdc913dca",
        "131c22db-3591-4fc8-a305-55444fa5ccd3",
        "132fe41d-f2f5-4de4-94d6-eed2f9b1ee69",
        "1377c8c8-4806-44d3-8fa4-9c6c686476b8",
        "13937bba-652e-4c46-b222-3003f4d1ff97",
        "13b33ae5-65a2-4eba-8a13-1dc1c8f96349",
        "13d54852-ae25-4f0b-823a-b09eea89f431",
        "17c60490-cd31-4f96-a4a2-2bb866b194a2",
        "18986625-3683-4b9a-b1cb-24f589571880",
        "18f36947-75b0-49fb-8d1c-29584a55cac5",
        "19f24784-ea9e-4a80-8b7f-63aadd968df0",
        "1a2ecf95-35f3-43b6-baf2-99a366172251",
        "1a6fcee6-0816-469b-acac-fe7ef2e87b83",
        "1b2fb645-dfbb-4d81-820a-644da7587cad",
        "1b4b9277-94d2-4a43-b25b-c16112bec073",
        "1c0ae35a-e2ec-4592-8e08-c40884656fa5",
        "1caee58f-eb14-4a6b-9339-1fe2ddf6692b",
        "1dc9a35f-3b69-4be1-8153-51d00c75f2b4",
        "1e196ed8-b66b-40b5-b62a-55ad2b74c147",
        "1e2ca66a-c176-45ea-a877-e87f7231e0ee",
        "1e70cd27-4707-4589-8ec5-9bd20c472a46",
        "1fec8e78-bce4-4aaf-ab1b-5451cc387264",
        "200c6ece-ac60-40b3-9d0a-a3fcd12c8385",
        "20193dcd-10c6-4f00-ae67-218665cb10a6",
        "2188d31a-bd44-4cfa-ad27-4ce273ab44ab",
        "22098786-6e16-43cc-a27d-191a01a1e3b5",
        "23276d57-ff37-4ba3-8091-292a5270d801",
        "23c115ed-4d98-4ca6-834a-8b7211232804",
        "246bab59-8b8a-488e-b59e-fbcf85675a83",
        "25da2ce8-f27d-4988-beb2-b0b89c7ba248",
        "268761a2-03f3-40df-8a8b-c3db24145b6b",
        "26a7ee05-5602-4d76-a7ba-eae8b7b67941",
        "26c3e886-e9d1-4564-931f-9f29092b2291",
        "26f3b60a-68fc-4574-b3a1-4d14b283eec9",
        "270a343c-ce6e-4c7e-be25-f29abe0e089b",
        "27922004-5251-4030-b22d-91ecd9a37ea4",
        "27a08af2-bae3-45d8-a121-b778c264c13b",
        "28ec9756-deaf-48b2-84d5-a623b99af263",
        "29b0c7cc-8e60-4a02-80a4-654af7674db8",
        "2a0878aa-8be1-451b-af64-7acaeda4effe",
        "2a486b53-dbd2-49c0-a2bc-278bdfc30833",
        "2d40ef1a-f71b-4fa9-924e-ad34ae11dcc2",
        "2d4d3d8e-2be3-4bef-9f87-7875a61c29de",
        "2da878bd-fba2-4832-b433-6fc8dc61192c",
        "2ddafda0-d50a-404a-8b76-6ad99a0f7b84",
        "2edc1f24-8740-4ead-93b3-24626a446ebc",
        "2f72f1f3-56b2-458c-b475-14cb91917ef9",
        "30b65a76-e816-400d-95b0-cd21a53059e7",
        "313456ba-dbef-4c89-a74d-5763e8b532e6",
        "3138fe80-4087-4b04-80a6-8866c738028a",
        "31562ffa-75f1-4684-9a95-f9fb0f7cd675",
        "32a8cd42-8b8b-4b91-b2b1-433d2ab5af62",
        "32d4b5e5-7d33-4e7f-b073-f8cffbbb47a1",
        "336be6bf-83eb-47ad-93ef-32250063f88d",
        "33858cdd-8b16-4201-8490-dc180f17036e",
        "34a6da52-ff06-46ec-b992-57c6f5f1f7a1",
        "34e6983d-a05d-4ac1-a1ca-8e3552d465bb",
        "37ccb541-667a-4428-947a-f5dda698fa3c",
        "394866fc-eedb-4f01-8536-3ff84b16be2a",
        "394b32ed-4e78-4d38-bd66-f6bb62bd5127",
        "398c4a51-ecc4-48b9-a08d-e7b7a9da2bc9",
        "3b2def9b-c603-4869-9919-f391fc256489",
        "3b2e5a14-128d-48aa-b581-482aac616d32",
        "3bcb3a4a-1aa5-4776-8993-ee8a11a17189",
        "3c896ded-22c5-450f-91f6-3d1ef0848f6e",
        "3d03fb76-0152-4719-a268-4b13a1a8c056",
        "3d6dba7a-5d59-483f-8929-2e4f0c2523cd",
        "3f758bb0-1c98-4fd8-b51a-94b29f9a3088",
        "40ce9886-40f7-4bc0-93aa-268a1b15d9bb",
        "4125a8ba-a96a-4839-8cb5-39d4234e0aa6",
        "4345a7b9-9a63-4910-a426-35363201d503",
        "441509e5-a165-4363-8ee7-bcf0b7d26739",
        "445112211283-2l4cqfgb0nqep0bu135v5au",
        "445112211283-sk04feuogpcjd3dq8eshrdn",
        "448dd786-dd9c-4775-8d97-d8feea676f01",
        "449a0ce2-795f-44f6-9e3d-c0fcf47b41b1",
        "450987b3-a09a-4f14-9b2c-4f301d1e15f5",
        "45a330b1-b1ec-4cc1-9161-9f03992aa49f",
        "45e28dcf-2a76-45e8-8b92-d95797007164",
        "4652a90b-39ba-4485-8f5d-33d3311b786b",
        "48af08dc-f6d2-435f-b2a7-069abd99c086",
        "4a31f64a-9afd-4dcb-a967-c4964c822db3",
        "4a3afd06-fa56-4e64-a5a3-c3b728345247",
        "4b172adc-e271-4247-b671-a405053bf382",
        "4c565139-8fec-473e-87d4-7f8dbcf10e9e",
        "4d2bd0ca-897a-4ae8-bfd9-c0df1cd739ff",
        "4d5d0954-2b14-42f8-8485-e2ded65482be",
        "4e445925-163e-42ca-b801-9073bfa46d17",
        "4f0f0b49-3478-482b-aa66-ddb2c5c2432a",
        "4f56b125-64e0-4857-ae99-45ddfb77f4d5",
        "501d6dd3-1f78-4bff-9115-ad2c6375b205",
        "506e08f9-6e9a-4da4-942b-a099a03644ae",
        "507964f9-00b0-4e2d-bc54-c8ad9d53594c",
        "51454d81-5e4a-45e1-874b-93615634ecf3",
        "5225545c-3ebd-400f-b668-c8d78550d776",
        "533cbd6f-6099-4eea-8f66-056a7ef8e8f1",
        "55b87f68-81e2-45b8-9da1-475c2003e9c1",
        "55f60dcc-faa4-466e-a109-9817be3450a4",
        "56046500-9c71-4bcd-888f-bbd7a465cda7",
        "56fb6ce1-4571-47bd-8949-d7ca89891f46",
        "5712d3fd-8e22-4040-afbf-70fa18f63627",
        "574df941-661b-4bfc-acb0-0a07de7de341",
        "5754bb84-f388-44f0-b3f7-9233a05bbb34",
        "57da3f69-2d82-4c17-9e57-2e6d78b2dc60",
        "580b37da-725a-4fee-8ef4-90f401fbe2c7",
        "58ea322b-940c-4d98-affb-345ec4cccb92",
        "59836d5a-09e3-407f-b5d1-2004a5810c4e",
        "5ad0e2f2-3f63-4ef9-a0d7-3d227c5ad94f",
        "5b4651ea-c56b-4e7c-acec-982882c3422a",
        "5b68579f-58a3-4dc3-a217-0c08b284fae2",
        "5bf16106-2b0e-42cc-9d42-a51fc8f56cfc",
        "5c2e880d-44a2-43b7-b50a-a733b8021b24",
        "5c34faf7-98ce-4786-a380-5600f1f8e80c",
        "5eae6ac8-1c04-432d-9a8e-dd3c88306f2d",
        "60c8bde5-3167-4f92-8fdb-059f6176dc0f",
        "610c30b3-95e2-41da-b4bd-2cd0cd9716f6",
        "61109738-7d2b-4a0b-9fe3-660b1ff83505",
        "6208afad-753e-4995-bbe1-1dfd204b3030",
        "63224634-e46c-47db-921f-42bf5bfeaf4e",
        "64f79cb9-9c82-4199-b85b-77e35b7dcbcb",
        "65578373-ca62-45fe-99e5-282766b6755c",
        "65d7ff01-3771-4724-8b9f-c5cbe87a80ce",
        "66a88757-258c-4c72-893c-3e8bed4d6899",
        "6746ed99-5ea6-40a0-8d5c-e515939c0b59",
        "67b0c2f4-650e-46b5-b90a-2583daee0bac",
        "67c2c056-e9cd-4a66-a045-e6b181034ae3",
        "67e3df25-268a-4324-a550-0de1c7f97287",
        "68a83d7f-9884-4f7f-8d5e-02fc737a6210",
        "6a9b9266-8161-4a7b-913a-a9eda19da220",
        "6cd30189-6916-4baf-9898-f371c90a0b82",
        "6cd5135b-7a07-43ec-acc2-b5e27dc628cb",
        "6d841955-8c7e-41e5-ab75-203d62e5b927",
        "6e4abb04-e406-4fef-944a-8978eb79dbe2",
        "6f8e7c96-93d0-4689-9813-c0541f3a4d99",
        "6ff46a19-6c2d-423e-8c7c-d2396ca915b4",
        "701d6dd3-1f78-4bff-9115-ad2c6375b205",
        "707aa1ac-be0a-478d-9ce7-0d2765a5c1d6",
        "70ae0d6c-910a-417e-a3ee-8fc28f19af8f",
        "70b19044-b12f-43ae-b7d5-4a435f4444d3",
        "717b5694-946e-4f2e-92d2-1b505dc7b536",
        "71a7c376-13e6-4100-968e-92ce98c5d3d2",
        "73323cc0-249f-4d0e-9030-080dfd793498",
        "733a4e29-9f27-4c7e-95c2-7a13bb7a99cf",
        "733d7a8b-5e7d-46d3-8001-c07e61887283",
        "73422220-546f-4dfe-a729-cc743e5cc603",
        "74c965fb-23b9-4bf1-b869-318bc0a777c5",
        "7557eb47-c689-4224-abcf-aef9bd7573df",
        "766ef332-38e5-4cb4-920c-baa478e39fd9",
        "767f3f15-c662-4f58-8979-fd41d589afd5",
        "777fc3d9-7c3e-4aba-84f8-3f92f38d8637",
        "780654d3-ab49-456f-98e9-76dd420ae69d",
        "7867c926-60f6-4172-b56d-d26ea10ea5e3",
        "78e7bc61-0fab-4d35-8387-09a8d2f5a59d",
        "79c1e2e1-e33a-4397-a416-9c683ce00887",
        "79efa807-5d5c-4067-9652-2909992724c2",
        "7aa99704-419c-4c72-ae2d-93ba801f40c3",
        "7ab7862c-4c57-491e-8a45-d52a7e023983",
        "7ae974c5-1af7-4923-af3a-fb1fd14dcb7e",
        "7c75e20a-efe5-4573-98ee-1d8f347e8461",
        "7d444187-6b6c-4810-8415-4664da24e337",
        "7df45227-d64b-4ebd-838e-f5f59d098a2b",
        "7e75663e-d29b-445a-9019-1c0d39f5f520",
        "7ead12ce-1eac-4dd3-8ce8-cb82e79dbea6",
        "7fa1636a-7f58-45ef-a91e-faa456872cd3",
        "80298f8f-d81a-4889-b87a-f4056128e2cc",
        "80ccca67-54bd-44ab-8625-4b79c4dc7775",
        "812fcd2a-bd10-44fa-8608-fd56e4c001e3",
        "820d2465-2336-4d5e-9e90-2b8ac685dec6",
        "8221f3ea-76ed-42d0-9dc2-7a308ed7e8a6",
        "82d8ab62-be52-a567-14ea-1616c4ee06c4",
        "83d396f9-90be-4200-a2b9-06684cfd0b55",
        "84b6d029-7858-466f-8d8f-2172cfe22832",
        "85447b25-b6aa-46e3-b27a-15deb03de2c3",
        "86614a7a-f3c1-43a9-bbe0-7c5b90c0a94a",
        "8824d12e-2a98-432f-9405-fd7e3fa04681",
        "88fe7e92-8134-46c4-a9cf-830afdb766b2",
        "89de8276-d29f-473d-a1f9-65e4bd4db633",
        "8ae7605a-24db-423c-b9c0-2f3d278a5069",
        "8b3391f4-af01-4ee8-b4ea-9871b2499735",
        "8c22b648-ee54-4ece-a4ca-3015b6d24f8e",
        "8ce3d22d-e8fa-4374-bff5-c9f3ffc48f77",
        "8dfa1904-3740-4636-bce3-cd5710b666d3",
        "8e5d5464-aa9d-49b6-b5e4-6cd12060da36",
        "900b11d6-b26f-4dc6-96c0-15f59cd1128f",
        "9028e929-9f68-47a4-802f-ccff933bbd10",
        "91ca2ca5-3b3e-41dd-ab65-809fa3dffffa",
        "923bb120-84d1-4a53-8c5b-bf0221e94bc7",
        "93357dba-c01a-4e1d-9b2c-248cd47a0f0f",
        "93d74522-5e1d-444d-81f2-653fb251230d",
        "943a2f1e-1a04-48b6-88d6-232c32fe2873",
        "94c63fef-13a3-47bc-8074-75af8c65887a",
        "98fbeb14-b5eb-4ffe-8dea-f150e02b6458",
        "996def3d-b36c-4153-8607-a6fd3c01b89f",
        "9a56f30e-f57c-4509-a703-8495e4c267f9",
        "9b11219c-f897-4393-b2d1-416e8da2dc36",
        "9b32af2c-cf92-4969-a3c0-a13458d16630",
        "9bc5ff82-e485-47eb-8b71-9f0df3fde5ee",
        "9bdb0045-3587-47f9-863a-2ca58d11e2e8",
        "9cc1263f-c84d-4983-9a44-90d066d5f3e0",
        "9d06afd9-66c9-49a6-b385-ea7509332b0b",
        "9d98f512-6670-420c-8d6e-a7982a6e5fc2",
        "9db66eb3-a3c4-4520-980b-b9a05fdc1b67",
        "9dc671c7-7f96-4764-b4e3-087bc7a434c2",
        "9e4a5442-a5c9-4f6f-b03f-5b9fcaaf24b1",
        "9ea1ad79-fdb6-4f9a-8bc3-2b70f96e34c7",
        "9f4ca253-cda6-4abf-bcea-0b74a3b1341b",
        "9fb94e2d-10e2-43c7-ac2a-becb1f8d3c11",
        "9fd48e9a-6178-463f-a1fd-e0d13e5367cb",
        "a196180e-f7e5-467b-b54b-125378c9aa08",
        "a1e44c57-210e-4b8d-aebe-61b7ee28399c",
        "a2177ee2-e7e8-406a-996d-9ac7ddaeb087",
        "a2e1c012-cc5b-412a-934b-03b294ae4b05",
        "a316900a-3c6b-4397-a6b0-ceab316a30a8",
        "a37563e7-04b1-47e6-913e-6a818fe4a82c",
        "a40d7d7d-59aa-447e-a655-679a4107e548",
        "a5e21429-5a73-c42a-4795-6760a5174361",
        "a7003abb-7b7d-4453-a985-354ab95bfc99",
        "a7859ceb-965d-4621-97c4-2173b1f887db",
        "a7cd46df-a6ca-44b7-9e5d-17e798a1f51d",
        "a9772429-3099-40f1-b912-e048566ddf32",
        "a9fb4803-d855-403f-90ae-64fbd954a54e",
        "aa432d76-2603-4e5c-90b1-b2a17cf56ab0",
        "aa9ecb1e-fd53-4aaa-a8fe-7a54de2c1334",
        "abdbc362-8f82-47a6-beba-d0d4427abdb9",
        "ac4c5372-dbef-4678-96e3-be2de39d4feb",
        "ad36c6d6-d3bf-470d-9f73-abf37c1367fa",
        "ad815f5e-5a00-416c-a922-74857be92f66",
        "ae123859-5065-4da9-96a5-713b7285c95d",
        "ae8c6990-e387-4d86-b0cb-b3243ea01294",
        "ae8e128e-080f-4086-b0e3-4c19301ada69",
        "aeaba202-1621-4230-94e9-bc3dc26b9d56",
        "aef1da84-8bad-454e-979e-8cd803517033",
        "af08ce07-1df9-4a39-b6e6-092ac934eb21",
        "afe4b91e-9617-4b42-8f60-97bd56f03e2d",
        "b0e203d2-05f8-4377-8c55-b060a6effc58",
        "b3263035-e5b4-418e-a2da-8ad6c880fad6",
        "b3320a4c-bdca-4f9d-9571-969623b15807",
        "b4114287-89e4-4209-bd99-b7d4919bcf64",
        "b49a0f1d-ffb8-4094-8396-d5115b412491",
        "b4c3dd28-8c53-4363-9f7a-477471be7fb8",
        "b562df6e-def0-4b08-b811-5ffc349bab30",
        "b598b8e8-c984-45cc-a63f-3a861c33dd03",
        "b669c6ea-1adf-453f-b8bc-6d526592b419",
        "b672c5ff-e5e1-4d30-a6ce-b38a43ac2b3d",
        "b6a5e632-5250-4590-8952-27e5e7b8ce9f",
        "b6b84568-6c01-4981-a80f-09da9a20bbed",
        "b70defd5-0df5-49e6-a560-28560a4103d4",
        "b7912db9-aa33-4820-9d4f-709830fdd78f",
        "b7b2acea-e78d-4280-91fe-c0558d920a7c",
        "b7bf0456-8c41-4efc-a2a9-d043812812f9",
        "b8ea1eae-ffd3-45b1-9629-6ecf07e86a17",
        "b97b6bd4-a49f-4a0c-af18-af507d1da76c",
        "ba23cd2a-306c-48f2-9d62-d3ecd372dfe4",
        "bb688920-6df7-4529-911c-dd7135237a63",
        "bb700ee6-c336-43ea-9a9c-95c70f784b82",
        "bbd92aa4-6315-4263-a34d-22e61f266a37",
        "bd73a70d-748a-4b87-9fc1-91c5ddcc32bc",
        "bfc0eb37-49a0-43ba-ad19-ec90f437f6b2",
        "c0716ee4-caf5-44a9-98a2-6f8721f4c851",
        "c087a482-1b91-449a-987d-88c03740e606",
        "c26550d6-bc82-4484-82ca-ac1c75308ca3",
        "c26cea16-ef6f-4e84-83b5-1925fa4cd57f",
        "c31ef04c-c358-4e2c-a529-5bb77f5fb8e9",
        "c606301c-f764-4e6b-aa45-7caaaea93c9a",
        "c64785ab-16a5-4aea-b25d-8dcf0b0036ef",
        "c65071a8-8d1f-4287-a8be-fa4a5e8a30d6",
        "c80d9aba-e613-43f2-8772-2061f3cd0743",
        "c8badb51-b15a-4246-b796-87aad17ed954",
        "ca4c9d38-cab8-4bb0-a863-ce62aa754907",
        "ca88ffc3-102e-41fe-a216-fed0dff56476",
        "cad8ec7a-3629-47fa-9ba1-dd5790aa806e",
        "cb5b7de5-2ef8-4fb2-9600-9feadb91dc45",
        "cb8b7578-ad74-4538-8cbd-4eb12ccbbc22",
        "cbb63f0b-c2a8-4c43-b7c9-68158b8be470",
        "cbfd6b46-22cd-4d28-88f9-9ddb9223a35e",
        "cc15fd57-2c6c-4117-a88c-83b1d56b4bbe",
        "cc4148f5-479d-402b-a4c0-a9211e922152",
        "ccdcebdf-92a2-4257-8ffa-55d008f7108d",
        "cde6e7d2-3e88-4b97-a745-71143a27d3a0",
        "cf917ca4-72ff-4845-bdda-3958c8978232",
        "d0af33eb-626f-4d68-bc7d-7fb35958e8a1",
        "d0f60e27-7247-45bc-8eac-ed75654dd4ca",
        "d32c68ad-72d2-4acb-a0c7-46bb2cf93873",
        "d3590ed6-52b3-4102-aeff-aad2292ab01c",
        "d35abca0-3f88-4171-a17f-4877f1d315cd",
        "d396de1f-10d4-4023-aae2-5bb3d724ba9a",
        "d3db5f78-8121-422c-a465-e5d3fe498028",
        "d42d7cf0-c05f-4317-8795-94949f776a53",
        "d45086c2-265d-4244-b501-0e8498d5d3fb",
        "d47222ee-36f3-4d76-910d-62a48c0a8c1c",
        "d53ed74d-b5d7-4ba5-ae29-e69ebbd8bb00",
        "d53f73f2-0588-4794-bb35-205b715787c7",
        "d5786915-2bb5-4c66-8b34-358e270469cb",
        "d59680c7-863a-4568-82b6-e298ea8127ac",
        "d5bbf704-0997-4e49-893e-61cf8a9ca3cd",
        "d71dfe16-1070-48f3-bd3a-c3ec919d34e7",
        "d7856b26-5baf-4a88-b9b4-7319cd42aa6a",
        "d7db2a1c-c38b-4bd1-a30f-0915167ba928",
        "d82073ec-4d7c-4851-9c5d-5d97a911d71d",
        "d8c17dbc-a54d-4a70-a508-ee629d9aae95",
        "d92fe772-5bd5-4d05-bb77-780eb82ae0b7",
        "d990aedd-6778-a49f-0336-b77a34183691",
        "d9991dac-e471-44b3-a593-54124183a4aa",
        "d9b8ec3a-1e4e-4e08-b3c2-5baf00c0fcb0",
        "d9df1e75-994f-4508-9f5e-cef78133e601",
        "da5335d8-f8f7-4bed-92c8-4aaeaaaaa26e",
        "db7de2b5-2149-435e-8043-e080dd50afae",
        "dbc06351-4e57-4235-a9aa-2fb87f193f1f",
        "dbfe7f07-d7e6-4e0b-b4dd-420a186d3af3",
        "dcad865d-9257-4521-ad4d-bae3e137b345",
        "ddc0c79c-ca80-42ea-9c91-c289e0dc9f67",
        "ddd72880-1013-4046-add1-ef606c793e3f",
        "de8bc8b5-d9f9-48b1-a8ad-b748da725064",
        "df09ff61-2178-45d8-888c-4210c1c7b0b2",
        "df3640a4-a074-41c1-80bb-096c0b0e05ce",
        "dfe74da8-9279-44ec-8fb2-2aed9e1c73d0",
        "e1913be8-1b52-43c0-9a5a-472698adc4fa",
        "e1cb9997-e27a-4cc3-bff2-490e2273ee80",
        "e3a69259-635c-4400-80cf-65f393fff8d7",
        "e3e1eece-6490-4979-a40a-12de6cbbc404",
        "e402651b-32ee-44cd-96dc-2087155905ce",
        "e475023c-fe81-4d3d-ab79-835459c90894",
        "e48d4214-364e-4731-b2b6-47dabf529218",
        "e4c55406-ad80-4c9c-8b61-17d36ddbcb64",
        "e4dd98a8-1ce3-42f5-a03c-3b077dac9264",
        "e5a18d7d-7b53-45ca-a2b8-a4e51677a34c",
        "e5b70556-0070-4dab-8e49-fb18f05cd3dd",
        "e6739753-13b7-4346-b3a1-328d31658549",
        "e689338d-57e3-418a-acfd-3cfeb2e74c13",
        "e69865b5-8b92-4abc-b5bc-ce0b2f38366a",
        "e69932cd-f814-4087-8ab1-5ab3f1ad18eb",
        "e79ecd92-93ad-4e37-b885-e547f4fb2647",
        "e800bb52-0c24-4d21-a3a2-0550d6818e32",
        "e8944f2a-db90-4f6a-acfe-bc132598efc8",
        "e8bdeda8-b4a3-4eed-b307-5e2456238a77",
        "e9c51622-460d-4d3d-952d-966a5b1da34c",
        "eaeca7d3-6935-4a67-a544-e930f9db88a7",
        "eaf8a961-f56e-47eb-9ffd-936e22a554ef",
        "ec7a29cb-d866-4036-ac4e-10226de0ae34",
        "ed3ade47-92db-42c5-a3e9-0af39ba81771",
        "eef9ebc6-e178-463c-85f4-9d0d85cdcace",
        "ef47e344-4bff-4e28-87da-6551a21ffbe0",
        "ef954110-2470-4e36-afd8-a3ea4d6f4e4b",
        "f0208175-83ef-4750-9657-3dd37aa0709d",
        "f174cc18-a8d5-417b-9888-a83b0852840e",
        "f1813110-8bba-4f85-b9db-1311a5b69291",
        "f1d7b18c-1d0a-4839-ba97-2ebe653d3deb",
        "f23c0d9b-6e8b-41c8-be84-ae927c1a558c",
        "f36992c1-1f0e-43e2-b69f-b043050113a3",
        "f3c1ad03-75d5-4a89-a293-2266750d3154",
        "f426d59c-9419-4c61-8fed-d2da583488d7",
        "f53a36d0-dc9d-4e42-a1b3-452a804d62a6",
        "f60b12b3-5157-4fbf-8c20-7a9a45698527",
        "f66295bb-bccf-44c8-9200-f6b5f19d8236",
        "f71d8b63-ca1b-4965-b7d6-12cd129f8ae7",
        "f8d98a96-0999-43f5-8af3-69971c7bb423",
        "f9040eb6-95ee-4906-b65f-edd0d01fbb6f",
        "f9bd8b76-b3c5-47b3-928c-80cd8a446af7",
        "fabcc1ee-b3fe-4cdd-8b41-27057d00e37e",
        "fc4979e5-0aa5-429f-b13a-5d1365be5566",
        "fc84ba08-4c2c-4b4f-bdfb-a47c552f112d",
        "fc9c94dd-4812-4063-b435-b628db9417c5",
        "fcbe1709-1667-479e-902c-47edf6a869df",
        "fcd39eab-50d0-43f3-aafb-ae3d453276e7",
        "fde6d64e-601e-422c-a882-7a4c35789620",
        "fe217466-5583-431c-9531-14ff7268b7b3",
        "fec2c3b0-23f2-4260-88ba-2bcd20caef11",
        "ff76af7c-8557-49f9-952d-8e0ad8d9e7d5",
        "ffcb16e8-f789-467c-8ce9-f826a080d987",
    };

    public static readonly Dictionary<string, string> ClassicDictionary = new(StringComparer.OrdinalIgnoreCase);
    public static readonly FrozenOrdinalStringDictionary<string> FrozenOrdinalStringDict;
    public static readonly FrozenDictionary<string, string> FrozenDict;

#pragma warning disable S3963 // "static" fields should be initialized inline
#pragma warning disable CA1810 // Initialize reference type static fields inline
    static SampleData()
    {
        foreach (var s in _raw)
        {
            ClassicDictionary[s] = s;
        }

        FrozenOrdinalStringDict = ClassicDictionary.ToFrozenDictionary(true);
        FrozenDict = ClassicDictionary.ToFrozenDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
#pragma warning restore CA1810 // Initialize reference type static fields inline
#pragma warning restore S3963 // "static" fields should be initialized inline
}
